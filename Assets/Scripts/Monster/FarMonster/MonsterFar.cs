using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFar : MonsterBase
{
    #region MonsterShoot
    public float monsterShootTimer;
    public float monsterShootDelay = 0.7f;
    public float monsterReloadDelay = 2f;
    public bool isReloading = false;
    public bool isAttacking = false;
    public int loadedBullet;
    public int magazineSize=3;
    public GameObject monsterBullet;
    public float bulletSpeed = 7.5f;
    #endregion

    #region States
    public MonsterChaseState chaseState { get; private set; }
    public MonsterAttackState attackState { get; private set; }
    public float attackRange = 8.0f;
    public float distanceToPlayer;
    #endregion

    #region Navigate
    public UnityEngine.AI.NavMeshAgent agent;
    #endregion

    public override void Awake()
    {
        base.Awake();

        chaseState = new MonsterChaseState(stateMachine, player, this);
        attackState = new MonsterAttackState(stateMachine, player, this);
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
        stateMachine.currentState.Update();

        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        monsterShootTimer -= Time.deltaTime;
    }
    
    public override void Attack()
    {
        if (!isAttacking && !isReloading) Shoot();
    }
    
    public virtual void Shoot()
    {
        if(loadedBullet > 0 && monsterShootTimer < 0.0)
        {
            monsterShootTimer = monsterShootDelay;
            loadedBullet--;
            Vector3 dir = player.transform.position-transform.position;

            //Create Bullet
            var bulletGo = MonsterPool.instance.pool.Get();
            var bulletComponent = bulletGo.GetComponent<MonsterBullet>();
            bulletGo.transform.position = transform.position;
                
            bulletComponent.BulletInitialize(Quaternion.AngleAxis(Random.Range(-15, 15), Vector3.forward) * dir, bulletSpeed);
        } 
        else if (loadedBullet <= 0)
        {
            Reload();
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