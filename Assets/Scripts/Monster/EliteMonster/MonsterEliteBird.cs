using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEliteBird : MonsterBase
{
    #region MonsterShoot
    public float fireInterval = 0.2f;
    public float monsterReloadDelay = 2f;
    public bool isReloading = false;
    public bool isAttacking = false;
    public int loadedBullet;
    public int magazineSize=3;
    public float bulletAngle = 20;
    public GameObject monsterBullet;
    public int bulletNum;
    #endregion

    #region States
    public MonsterChaseStateBird chaseState { get; private set; }
    public MonsterEscapeStateBird escapeState { get; private set; }
    public MonsterAttackStateBird attackState { get; private set; }
    public float chaseRange = 20.0f;
    public float attackRange = 8.0f;
    public float escapeRange = 4.0f;
    public float distanceToPlayer;
    private Vector3 dir;
    #endregion

    #region Navigate
    public UnityEngine.AI.NavMeshAgent agent;
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

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        stateMachine.Initialize(chaseState);
    }

    public override void Update()
    {
        base.Update();
        //Debug.Log(stateMachine.currentState);
        stateMachine.currentState.Update();
        
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        if(loadedBullet<=0 && !isAttacking && !isReloading) Reload();
    }
    
    public override void Attack()
    {
        if (!isAttacking && !isReloading) Shoot();
    }
    
    public void Shoot()
    {
        isAttacking = true;
        if(loadedBullet > 0 && (!isReloading))
        {
            loadedBullet--;
            dir = player.transform.position - transform.position;
            
            StartCoroutine(SpawnBullets());
        }

        isAttacking = false;
    }

    IEnumerator SpawnBullets()
    {
        for (int j=0; j < bulletNum; j++)
        {

            for (int i=0; i < 6; i++)
            {
                var bulletGo = MonsterPool.instance.pool.Get();
                var bulletComponent = bulletGo.GetComponent<MonsterBullet>();
                bulletGo.transform.position = transform.position;
                // - new Vector3(0, 0.6f, 0);
                
                bulletComponent.BulletInitialize(Quaternion.AngleAxis(bulletAngle*(i-2.5f), Vector3.forward) * dir);
            }
            yield return new WaitForSeconds(fireInterval);
        }
        
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

    public void SpeedToZero()
    {
        agent.speed = 0;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = 0;
    }

    public void SpeedReturn()
    {
        agent.speed = moveSpeed;
    }

}