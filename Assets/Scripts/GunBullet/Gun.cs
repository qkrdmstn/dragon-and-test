using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Gun : MonoBehaviour
{
    #region Components
    private SpriteRenderer renderer;
    #endregion

    [Header("Gun Timer")]
    public float shootTimer;

    [Header("Gun Data")]
    public GunData initData;
    public int damage = 1;
    public float shootDelay;
    public float reloadTime;
    public int maxBullet;
    public int magazineSize;
    public int loadedBullet;
    public float maxRecoilDegree;
    public float recoilIncrease;
    public float bulletSpeed;
    public float range;
    public float knockbackForce;
    public bool isAutomatic;
    public GameObject gunPrefab;

    [Header("Bullet Prefabs")]
    public GameObject bulletPrefab;

    [Header("Gun State")]
    [SerializeField] protected bool isReloading = false;
    [SerializeField] protected int continuousShootCnt = 0;

    [Header("CameraSetting")]
    // 플레이어가 총을 쐈을때 필요한 카메라 반동 쉐이킹 
    public CamShakeProfile profile;
    public CameraManager cameraManager;
    protected CinemachineImpulseSource impulseSource;

    [Header("UI")]
    public Image reloadUIImg;

    private void Awake()
    {
        renderer = gameObject.GetComponent<SpriteRenderer>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        if (Player.instance.isCombatZone)
            cameraManager = FindObjectOfType<CameraManager>();

        GameObject reloadUI = UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[6];
        reloadUIImg = reloadUI.GetComponent<Image>();
    }

    private void Update()
    {
        shootTimer -= Time.deltaTime;

        if (Player.instance.isAttackable && !isReloading)
        {
            if (loadedBullet == 0 && Input.GetKeyDown(KeyCode.Mouse0))
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
            if (!isReloading)
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
        if(isReloading)
        {
            isReloading = false;
            reloadUIImg.gameObject.SetActive(false);

            //Reload Visulaize
            renderer.color = new Color(1, 1, 1);
            continuousShootCnt = 0;
        }
    }

    public void InitGunData(GunData _data)
    {
        damage = _data.damage;
        shootDelay = _data.shootDelay;
        reloadTime = _data.reloadTime;
        maxBullet = _data.maxBullet;
        magazineSize = _data.magazineSize;
        loadedBullet = _data.loadedBullet;
        maxRecoilDegree = _data.maxRecoilDegree;
        recoilIncrease = _data.recoilIncrease;
        bulletSpeed = _data.bulletSpeed;
        range = _data.range;
        knockbackForce = _data.knockbackForce;
        isAutomatic = _data.isAutomatic;

        gunPrefab = _data.gunPrefab;
        bulletPrefab = _data.bulletPrefab;
    }

    protected virtual void Shoot()
    {
        if(loadedBullet > 0 && shootTimer < 0.0)
        {
            Player.instance.animController.isBreath = true;
            if (cameraManager != null)
            {   // player 카메라 총 반동
                cameraManager.CameraShakeFromProfile(profile, impulseSource);
            }

            //Shoot Setting
            shootTimer = shootDelay;
            if(SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.SR46))
            {
                shootTimer -= shootDelay * SkillManager.instance.GetSkillDB(SeotdaHwatuCombination.SR46).probability;
            }
            loadedBullet--;
            continuousShootCnt++;
            SoundManager.instance.SetEffectSound(SoundType.Player, PlayerSfx.Breath);

            int bulletDamage = damage + Player.instance.reinforceAttack;
            float bulletScale = 1.0f;
            //장삥
            //10%로 데미지 1 증가
            SkillDB jpp110Data = SkillManager.instance.GetSkillDB(SeotdaHwatuCombination.JPP110);
            float randomVal = Random.Range(0.0f, 1.0f);
            if (SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.JPP110) && randomVal <= jpp110Data.probability)
            {
                bulletDamage += jpp110Data.damage;
                bulletScale = 1.5f;
            }

            //Create Bullet
            Vector2 dir = GetShootingDirection();
            float theta = Vector2.Angle(Vector2.right, dir);
            if (dir.y < 0)
                theta *= -1;
            GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, theta + 90));
            PlayerNormalBullet bullet = bulletObj.GetComponent<PlayerNormalBullet>();

            bullet.BulletInitialize(bulletDamage, range, bulletSpeed, knockbackForce, dir, bulletScale);
            StartCoroutine(InactiveIsAttacking());

            //Gun Inventory Update
            GunManager.instance.UpdateCurrentGunBulletData(maxBullet, loadedBullet);
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
        if(loadedBullet != magazineSize && maxBullet > 0)
        {
            Player.instance.animController.isBreath = false;

            isReloading = true;

            //장전 UI 활성화 및 위치 설정
            reloadUIImg.gameObject.SetActive(true);
            Vector3 uiPos = Player.instance.transform.position + Vector3.up * 0.8f;
            reloadUIImg.rectTransform.position = Camera.main.WorldToScreenPoint(uiPos);

            StartCoroutine(ReloadProcess());
        }
    }

    IEnumerator ReloadProcess()
    {
        float timer = 0.0f;
        while (timer <= reloadTime)
        {
            timer += Time.deltaTime;
            reloadUIImg.fillAmount = timer / reloadTime;

            //장전 UI 위치 설정
            Vector3 uiPos = Player.instance.transform.position + Vector3.up * 0.8f;
            reloadUIImg.rectTransform.position = Camera.main.WorldToScreenPoint(uiPos);
            yield return new WaitForFixedUpdate();
        }
        int reloadBulletSize = magazineSize - loadedBullet;
        if(reloadBulletSize <= maxBullet)
        {
            loadedBullet = magazineSize;
            if(maxBullet < 10000) //총알 무한대
                maxBullet -= reloadBulletSize;
        }
        else
        {
            loadedBullet += maxBullet;
            maxBullet -= maxBullet;
        }
        isReloading = false;

        reloadUIImg.gameObject.SetActive(false);
        //Gun Inventory Update
        GunManager.instance.UpdateCurrentGunBulletData(maxBullet, loadedBullet);
    }
}
