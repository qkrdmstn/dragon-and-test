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
    public int loadedBullet;
    public int magazineSize=3;
    public GameObject monsterBullet;
    #endregion

    #region States
    public MonsterChaseState chaseState { get; private set; }
    public MonsterChaseAttackState chaseAttackState { get; private set; }
    public MonsterAttackState attackState { get; private set; }
    public float attackRange = 5.0f;
    public float haltRange = 2.0f;
    #endregion

    public Chase chase;
    public MonsterPool monsterPool;
    public float distanceToPlayer;
   
    public override void Awake()
    {
        base.Awake();
        chaseState = new MonsterChaseState(stateMachine, player, this);
        chaseAttackState = new MonsterChaseAttackState(stateMachine, player, this);
        attackState = new MonsterAttackState(stateMachine, player, this);
    }

    public override void Start()
    {
        base.Start();
        stateMachine.Initialize(chaseState); //state initialize

        chase = GetComponent<Chase>(); //get component of this code
        monsterPool = GetComponent<MonsterPool>();
    }

    public override void Update()
    {
        base.Update();
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
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

            //Create Bullet
            GameObject bulletObj = Instantiate(monsterBullet, transform.position, transform.rotation);
            Rigidbody2D rigid = bulletObj.GetComponent<Rigidbody2D>();
            MonsterBullet bullet = bulletObj.GetComponent<MonsterBullet>();

            Vector2 dir = player.transform.position - transform.position; 
            dir.Normalize();

            bullet.BulletInitialize(damage, dir);
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

}