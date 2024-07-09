using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class Gun : MonoBehaviour
{
    #region Components
    private Player player;
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
    public GameObject gunPrefab;

    [Header("Bullet Prefabs")]
    public GameObject bulletPrefab;

    [Header("Gun State")]
    [SerializeField] private bool isReloading = false;
    [SerializeField] private bool clickReloadFlag = false;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private int continuousShootCnt = 0;

    [Header("CameraSetting")]
    // 플레이어가 총을 쐈을때 필요한 카메라 반동 쉐이킹 
    public CamShakeProfile profile;
    public CameraManager cameraManager;
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
       
       // player = GameObject.FindWithTag("Player").GetComponent<Player>();
        renderer = gameObject.GetComponent<SpriteRenderer>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        player = GameManager.instance.player;
        if (player.isCombatZone)
            cameraManager = FindObjectOfType<CameraManager>();
    }

    private void Update()
    {
        shootTimer -= Time.deltaTime;

        if (player.isAttackable && !isReloading)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0) && loadedBullet == 0)
                clickReloadFlag = true;

            if (clickReloadFlag && Input.GetKeyDown(KeyCode.Mouse0))
                Reload();
            else if (Input.GetKey(KeyCode.Mouse0))
                Shoot();

            if (Input.GetKeyDown(KeyCode.R))
                Reload();
        }
        else if (!player.isAttackable && player.stateMachine.currentState == player.dashState)
        {
            if (!isReloading)
            {
                if (Input.GetKeyDown(KeyCode.R))
                    Reload();
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
            continuousShootCnt = 0;

        player.anim.SetBool("isAttacking", isAttacking);
    }

    private void OnDisable()
    {
        if(isReloading)
        {
            isReloading = false;
            clickReloadFlag = true;
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

        gunPrefab = _data.gunPrefab;
        bulletPrefab = _data.bulletPrefab;
    }

    public void Shoot()
    {
        if(loadedBullet > 0 && shootTimer < 0.0)
        {
            if(cameraManager != null)
            {   // player 카메라 총 반동
                cameraManager.CameraShakeFromProfile(profile, impulseSource);
            }

            //Shoot Setting
            shootTimer = shootDelay;
            loadedBullet--;
            isAttacking = true;
            continuousShootCnt++;
            SoundManager.instance.SetEffectSound(SoundType.Player, PlayerSfx.Breath);

            //Create Bullet
            Vector2 dir = GetShootingDirection();
            float theta = Vector2.Angle(Vector2.right, dir);
            if (dir.y < 0)
                theta *= -1;
            GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, theta));
            Bullet bullet = bulletObj.GetComponent<Bullet>();

            bullet.BulletInitialize(damage, dir);
            StartCoroutine(InactiveIsAttacking());

            //Gun Inventory Update
            GunManager.instance.UpdateCurrentGunBulletData(maxBullet, loadedBullet);
        }
    }
    
    private Vector2 GetShootingDirection()
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

    IEnumerator InactiveIsAttacking()
    {
        yield return new WaitUntil(() => !Input.GetKey(KeyCode.Mouse0));
        yield return new WaitForSeconds(shootDelay);
        
        if (!Input.GetKey(KeyCode.Mouse0))
        {
            isAttacking = false;
        }
    }

    public void Reload()
    {
        if(loadedBullet != magazineSize)
        {
            //Debug.Log("Reload Start");
            clickReloadFlag = false;
            isAttacking = false;
            isReloading = true;

            //Reload Visulaize
            renderer.color = new Color(1, 0, 0);
            StartCoroutine(ReloadProcess());
        }
    }

    IEnumerator ReloadProcess()
    {
        yield return new WaitForSeconds(reloadTime);
        loadedBullet = magazineSize;
        isReloading = false;
                    
        renderer.color = new Color(1, 1, 1);

        //Gun Inventory Update
        GunManager.instance.UpdateCurrentGunBulletData(maxBullet, loadedBullet);
    }
}
