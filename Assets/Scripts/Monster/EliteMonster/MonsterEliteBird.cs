using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEliteBird : MonsterBase
{
    #region MonsterShoot
    public float monsterShootTimer;
    public float monsterShootDelay = 0.7f;
    public float monsterReloadDelay = 2f;
    public bool isReloading = false;
    public int loadedBullet;
    public int magazineSize=3;
    public GameObject monsterBullet;
    #endregion

    #region States
    public MonsterChaseStateBird chaseState { get; private set; }
    public MonsterEscapeStateBird escapeState { get; private set; }
    public MonsterAttackStateBird attackState { get; private set; }
    public float chaseRange = 20.0f;
    public float attackRange = 8.0f;
    public float escapeRange = 4.0f;
    public float distanceToPlayer;
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
        stateMachine.currentState.Update();
        
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        //Attack
        monsterShootTimer -= Time.deltaTime;
        
    }
    
    public override void Attack()
    {
        Shoot();
    }
    
    public void Shoot()
    {
        if(loadedBullet > 0 && monsterShootTimer < 0.0)
        {
            monsterShootTimer = monsterShootDelay;
            loadedBullet--;

            Vector3 dir = player.transform.position-transform.position;

            //Create Bullet
            for (int i=0; i < 3; i++)
            {
                var bulletGo = MonsterPool.instance.pool.Get();
                var bulletComponent = bulletGo.GetComponent<MonsterBullet>();
                bulletGo.transform.position = transform.position;
                
                bulletComponent.BulletInitialize(Quaternion.AngleAxis(30*(i-1), Vector3.forward) * dir);
            }
            
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