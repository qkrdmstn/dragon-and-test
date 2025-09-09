using System.Collections;
using UnityEngine;
using Cinemachine;

public class Gun : MonoBehaviour
{
    #region Components
    private SpriteRenderer renderer;
    #endregion

    [Header("Gun Timer")]
    public float shootTimer;

    GunItemData gunItemData;
    IGun gunInterface;
     public GunItemData initItemData
    {
        get
        {
            return gunItemData;
        }
        set
        {   // 설정과 동시에 초기화
            gunItemData = value;
            gunInterface = ItemManager.instance.gunController;

            GunData _data = value.gunData;
            damage = _data.damage;
            shootDelay = _data.shootDelay;
            reloadTime = _data.reloadTime;

            magazineSize = _data.magazineSize;
            originMaxBullet = _data.maxBullet;
            refMaxBullet = _data.maxBullet;
            refLoadedBullet = _data.loadedBullet;

            maxRecoilDegree = _data.maxRecoilDegree;
            recoilIncrease = _data.recoilIncrease;
            bulletSpeed = _data.bulletSpeed;
            range = _data.range;
            knockbackForce = _data.knockbackForce;
            isAutomatic = _data.isAutomatic;

            // Prefab Setting
            bulletPrefab = value.bulletPrefab;
        }
    }

    int originMaxBullet;

    #region CurGunDataValue
    public int damage = 1;
    public float shootDelay;
    public float reloadTime;

    public int magazineSize; // 한 번 장전 기준 총알수
    int maxBullet;
    public int refMaxBullet
    {
        get { return maxBullet; }
        set {
            maxBullet = value;
            gunInterface.OnBulletAction(maxBullet, -1);
        }
    }

    int loadedBullet;
    public int refLoadedBullet
    {
        get { return loadedBullet; }
        set
        {
            loadedBullet = value;
            gunInterface.OnBulletAction(-1, loadedBullet);
        }
    }
    public float maxRecoilDegree;
    public float recoilIncrease;
    public float bulletSpeed;
    public float range;
    public float knockbackForce;
    public bool isAutomatic;
    #endregion

    [Header("Prefabs")]
    public GameObject bulletPrefab;

    [Header("Gun State")]
    bool isReloading;
    protected bool refReloadState
    {
        get { return isReloading; }
        set
        {
            isReloading = value;
            gunInterface.OnReloadAction(isReloading);
        }
    }
    [SerializeField] protected int continuousShootCnt = 0;

    [Header("CameraSetting")]   // 플레이어가 총을 쐈을때 필요한 카메라 반동 쉐이킹 
    public CamShakeProfile profile;
    public CameraManager cameraManager;
    protected CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        renderer = gameObject.GetComponent<SpriteRenderer>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        if (Player.instance.isCombatZone)
            cameraManager = FindObjectOfType<CameraManager>();
    }

    private void Update()
    {
        shootTimer -= Time.deltaTime;

        if (Player.instance.isAttackable && !refReloadState)
        {
            if (refLoadedBullet == 0 && Input.GetKeyDown(KeyCode.Mouse0))
                Reload();
            else if(isAutomatic && Input.GetKey(KeyCode.Mouse0))
                    Shoot();
            else if (!isAutomatic && Input.GetKeyDown(KeyCode.Mouse0))
                    Shoot();

            if (Input.GetKeyDown(KeyCode.R))
                Reload();
        }
        else if (!Player.instance.isAttackable && Player.instance.stateMachine.currentState == Player.instance.dashState)
        {
            if (!refReloadState)
            {
                if (Input.GetKeyDown(KeyCode.R))
                    Reload();
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
            continuousShootCnt = 0;

        //Player.instance.anim.SetBool("isAttacking", isAttacking);
    }

    private void OnDisable()
    {
        if(refReloadState)
        {
            refReloadState = false;

            //Reload Visulaize
            renderer.color = new Color(1, 1, 1);
            continuousShootCnt = 0;
        }
    }

    protected virtual void Shoot()
    {
        if(refLoadedBullet > 0 && shootTimer < 0.0)
        {
            Vector2 dir = GetShootingDirection();
            Player.instance.animController.isBreath = true;
            if (Player.instance.cameraManager != null)
            {   // player 카메라 총 반동
                Player.instance.cameraManager.CameraShakeFromProfile(profile, impulseSource, -dir);
            }

            //Shoot Setting
            shootTimer = CalcShootDelay();
            refLoadedBullet--;
            continuousShootCnt++;
            SoundManager.instance.SetEffectSound(SoundType.Player, PlayerSfx.Breath);

            int bulletDamage = damage + Player.instance.reinforceAttack;
            float bulletScale = 1.0f;
            //장삥
            //10%로 데미지 1 증가
            SkillDB jpp110Data = SkillManager.instance.GetSkillDB(SeotdaHwatuCombination.JPP110);
            float jpp110Prob = SkillManager.instance.GetSkillProb(SeotdaHwatuCombination.JPP110);
            float randomVal = Random.Range(0.0f, 1.0f);
            if (SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.JPP110) && randomVal <= jpp110Prob)
            {
                bulletDamage += jpp110Data.damage;
                bulletScale = 1.5f;
            }

            //Create Bullet
            float theta = Vector2.Angle(Vector2.right, dir);
            if (dir.y < 0)
                theta *= -1;
            GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, theta + 90));
            PlayerNormalBullet bullet = bulletObj.GetComponent<PlayerNormalBullet>();

            bullet.BulletInitialize(bulletDamage, range, bulletSpeed, knockbackForce, dir, bulletScale);
            StartCoroutine(InactiveIsAttacking());
        }
    }

    protected virtual Vector2 GetShootingDirection()
    {
        //Initial Direction Setting
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        dir.Normalize();

        //Recoil Setting
        float degree = Random.Range(-maxRecoilDegree, maxRecoilDegree);
        //Debug.Log(degree);
        Vector3 direction = Quaternion.AngleAxis(degree * continuousShootCnt * recoilIncrease, Vector3.forward) * dir;

        Vector2 result = direction;
        result.Normalize();
        return result;
    }

    public float CalcShootDelay()
    {
        float timer = shootDelay;
        if (SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.SR46))
            timer -= shootDelay * SkillManager.instance.GetSkillProb(SeotdaHwatuCombination.SR46);
        if(Player.instance.isSuperman)
            timer -= shootDelay * SkillManager.instance.GetSkillProb(SeotdaHwatuCombination.GTT38);

        return timer;
    }

    protected IEnumerator InactiveIsAttacking()
    {
        yield return new WaitUntil(() => !Input.GetKey(KeyCode.Mouse0));

        if (!Input.GetKey(KeyCode.Mouse0))
        {
            Player.instance.animController.isBreath = false;
        }
    }

    protected virtual void Reload()
    {
        if(refLoadedBullet != magazineSize && refMaxBullet > 0)
        {
            refReloadState = true;
        }
    }

    public void DoneReloadUI()
    {
        int reloadBulletSize = magazineSize - refLoadedBullet;
        if (reloadBulletSize <= refMaxBullet)
        {
            refLoadedBullet = magazineSize;
            if (refMaxBullet < 10000) //총알 무한대
                refMaxBullet -= reloadBulletSize;
        }
        else
        {
            refLoadedBullet += refMaxBullet;
            refMaxBullet -= refMaxBullet;
        }
        refReloadState = false;
    }

    public void ResetBulletCnt()
    {
        refLoadedBullet = magazineSize;
        refMaxBullet = originMaxBullet;
    }
}
