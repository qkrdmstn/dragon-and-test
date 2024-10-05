using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEliteBird : MonsterBase2
{
    #region MonsterShoot
    [Header("MonsterEliteBird ------------------")]
    public float fireInterval;
    public float monsterReloadDelay;
    public bool isReloading = false;
    public bool isAttacking = false;

    public GameObject monsterBullet;
    public int loadedBullet;
    public int magazineSize;
    public float bulletAngle;       // 부채꼴 사이각
    public float bulletSpeed;
    public int bulletsOnce;         // 최초 발사 총알 수
    public int bulletNum;           // total shoot count
    public float[] shootNumTheta;   // 지그재그...
    #endregion

    #region States
    public MonsterChaseStateBird chaseState { get; private set; }
    public MonsterEscapeStateBird escapeState { get; private set; }
    public MonsterAttackStateBird attackState { get; private set; }

    [Header("RangeByState")]
    public float chaseRange;
    public float attackRange;
    public float escapeRange;
    Vector3 dir;
    #endregion

    public override void Awake()
    {
        base.Awake();
        chaseState = new MonsterChaseStateBird(stateMachine, player, this);
        escapeState = new MonsterEscapeStateBird(stateMachine, player, this);
        attackState = new MonsterAttackStateBird(stateMachine, player, this);
    }

    public override void Start()
    {
        base.Start();

        stateMachine.Initialize(chaseState);
    }

    public override void Update()
    {
        base.Update();
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        stateMachine.currentState.Update();
    }
    
    public override void Attack()
    {
        if (!isAttacking && loadedBullet > 0 && !isReloading) Shoot();
    }
    
    public void Shoot()
    {
        isAttacking = true;
        if(loadedBullet > 0 && (!isReloading))
        {
            loadedBullet--;
            
            StartCoroutine(SpawnBullets());
        }
    }


    IEnumerator SpawnBullets()
    {
        int curbulletsOnce = bulletsOnce;
        float theta = bulletAngle / bulletsOnce;

        for (int j = 0; j < bulletNum; j++)
        {   
            dir = player.transform.position - transform.position;
            for (int i = 0; i < curbulletsOnce; i++)
            {
                var bulletGo = MonsterPool.instance.pool.Get();
                var bulletComponent = bulletGo.GetComponent<MonsterBullet>();
                bulletGo.transform.position = transform.position;

                bulletComponent.BulletInitialize(Quaternion.AngleAxis(theta * (i - shootNumTheta[j]), Vector3.forward) * dir, bulletSpeed);
            }
            curbulletsOnce--;
            yield return new WaitForSeconds(fireInterval);
        }
        if (loadedBullet <= 0 && !isReloading) Reload();
        isAttacking = false;
    }

    public void Reload()
    {
        isReloading = true;
        StartCoroutine(ReloadProcess());
    }

    IEnumerator ReloadProcess()
    {
        yield return new WaitForSeconds(monsterReloadDelay);

        loadedBullet = magazineSize;
        isReloading = false;
    }
}