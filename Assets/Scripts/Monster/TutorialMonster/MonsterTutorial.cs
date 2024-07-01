using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType
{
    Archer, // 매
    Warrior // 참새 
}

public class MonsterTutorial : MonsterBase
{
    public MonsterType myType;

    #region States
    public MonsterChaseStateTutorial chaseState { get; private set; }
    public MonsterAttackStateTutorial attackState { get; private set; }
    public float attackRange = 8.0f;
    public float distanceToPlayer;
    #endregion

    #region Navigate
    public UnityEngine.AI.NavMeshAgent agent;
    public bool isChase = true;
    #endregion

    public override void Awake()
    {
        stateMachine = new MonsterStateMachine();
        effectState = new MonsterEffectState(stateMachine, player, this);

        player = GameObject.FindWithTag("Player");

        chaseState = new MonsterChaseStateTutorial(stateMachine, player, this);
        attackState = new MonsterAttackStateTutorial(stateMachine, player, this);
    }

    public override void Start()
    {
        temp = GameObject.FindObjectOfType<temp>(); // score UI를 위한 스크립트
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // 추격 관련 진행중 -> 맵 베이크 알기
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        stateMachine.Initialize(chaseState);
    }

    public override void Update()
    {
        base.Update(); // 넉백에 대한 조건문 체크
        stateMachine.currentState.Update();

        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        monsterShootTimer -= Time.deltaTime;

        if (rigidBody.velocity.magnitude <= 0.1f)
        {
            rigidBody.velocity = Vector2.zero;
            isKnockedBack = false;
        }
    }


    public bool isAttacking = false;
    public override void Attack()
    {
        if (!isAttacking && !isReloading) Shoot();
    }

    public int loadedBullet;
    public float monsterShootTimer;
    public float monsterReloadDelay = 2f;
    public float monsterShootDelay = 0.7f;
    public int magazineSize = 3;

    public void Shoot()
    {
        if (loadedBullet > 0 && monsterShootTimer < 0.0)
        {
            monsterShootTimer = monsterShootDelay;
            loadedBullet--;
            Vector3 dir = player.transform.position - transform.position;

            //Create Bullet
            for (int i = 0; i < 3; i++)
            {
                var bulletGo = MonsterPool.instance.pool.Get();
                var bulletComponent = bulletGo.GetComponent<MonsterBullet>();
                bulletGo.transform.position = transform.position;

                bulletComponent.BulletInitialize(Quaternion.AngleAxis(Random.Range(-15, 15), Vector3.forward) * dir, Random.Range(3f, 7f));
            }

        }
        else if (loadedBullet <= 0)
        {
            Reload();
        }
    }

    public bool isReloading = false;
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

    public override void OnDamaged(int damage)
    {
        SoundManager.instance.SetEffectSound(SoundType.Monster, "Damage");

        switch (myType)
        {
            case MonsterType.Archer:
                if (Tutorial.killState == 0)
                {
                    Tutorial.killState = 1;
                    base.OnDamaged(damage);
                }
                else if(Tutorial.killState >= 2)
                {
                    base.OnDamaged(damage);
                }
                break;
            case MonsterType.Warrior:
                if(Tutorial.useSkill) 
                    base.OnDamaged(damage);
                break;
        }
    }

    // 튜토리얼 전용 죽음 -> spawner 불필요
    public override void Dead()
    {
        if (Tutorial.killState < 3) Tutorial.killState = 3;
        else Tutorial.deadCnt++;

        if (myType == MonsterType.Warrior) Tutorial.isWarriorDied = true;

        Destroy(gameObject);
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

    public void ChangeChaseState(bool state)
    {
        isChase = state;
    }
}
