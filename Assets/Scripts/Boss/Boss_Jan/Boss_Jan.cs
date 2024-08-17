using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public enum BossStates_Jan
{
    idle,
    chase,
    basicAttack,
    pattern1,
    pattern2,
    pattern3,
    spawnMonster,
    hit
}

public class Boss_Jan : Boss
{
    //Temp
    public BossStates_Jan initState;
    public BossStates_Jan curState;

    [Header("Chase State Info")]
    public float chaseRange;

    [Header("Attack State Info")]
    public float attackRange;

    [Header("Basic Attack State Info")]
    public GameObject bulletPrefab;
    public float shootDelay;
    public float reloadTime;
    public int magazineSize;
    public int loadedBullet;
    public float maxRecoilDegree;
    public float bulletSpeed;
    public int reloadCnt = 0;

    [Header("Pattern1 Info")]
    public float pattern1Prob = 0.5f;
    public float sphereShootNum;
    public float sphereInterval;
    public float sphereRandomInterval;

    public float randomShootNum;
    public float randomInterval;

    public float pattern1BulletSpeed;
    public float waveNum;

    [Header("Pattern2 Info")]
    public float pattern2Prob = 0.3f;
    public GameObject displayPrefab;
    public float pattern2Delay;
    public int pattern2ShootNum;
    public float pattern2ShootDelay;
    public float pattern2RangeDisplayTime;
    public int pattern2ShootOffset;
    public float pattern2AttackRange;
    public bool isPattern2;

    [Header("Pattern2 Info")]
    public float pattern3Prob = 0.2f;


    [Header("Spawn Monster State Info")]
    public GameObject[] spawnMosnterPrefabs;
    public BlockInfo bossField;
    public float spawnPeriod;
    public float spawnWaveCnt = 0;
    public float spawnDelay = 1.0f;
    public float spawnTimer;

    #region Componets
    #endregion

    #region States
    public BossStateMachine stateMachine;

    public BossIdleState_Jan bossIdleState;
    public BossChaseState_Jan bossChaseState;
    public BossBasicAttackState_Jan bossBasicAttackState;
    public BossPattern1State_Jan BossPattern1State;
    public BossPattern2State_Jan BossPattern2State;
    public BossPattern3State_Jan BossPattern3State;
    public BossMonsterSpawnState_Jan bossMonsterSpawnState;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        spawnTimer = spawnPeriod;
        bossField = FindObjectOfType<BlockInfo>();
        bossField.InitializeBlockInfo(0);

        stateMachine = new BossStateMachine();

        bossIdleState = new BossIdleState_Jan(this, stateMachine, player);
        bossChaseState = new BossChaseState_Jan(this, stateMachine, player);
        bossBasicAttackState = new BossBasicAttackState_Jan(this, stateMachine, player);
        BossPattern1State = new BossPattern1State_Jan(this, stateMachine, player);
        BossPattern2State = new BossPattern2State_Jan(this, stateMachine, player, bossField);
        BossPattern3State = new BossPattern3State_Jan(this, stateMachine, player);
        bossMonsterSpawnState = new BossMonsterSpawnState_Jan(this, stateMachine, player, bossField);

        switch (initState)
        {
            case BossStates_Jan.idle:
                stateMachine.Initialize(bossIdleState);
                break;
            case BossStates_Jan.chase:
                stateMachine.Initialize(bossChaseState);
                break;            
            case BossStates_Jan.basicAttack:
                stateMachine.Initialize(bossBasicAttackState);
                break;
            case BossStates_Jan.pattern1:
                stateMachine.Initialize(BossPattern1State);
                break;
            case BossStates_Jan.pattern2:
                stateMachine.Initialize(BossPattern2State);
                break;
            case BossStates_Jan.pattern3:
                stateMachine.Initialize(BossPattern3State);
                break;
            case BossStates_Jan.spawnMonster:
                stateMachine.Initialize(bossMonsterSpawnState);
                break;
            default:
                stateMachine.Initialize(bossIdleState);
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.currentState.Update();

        if (stateMachine.currentState == bossIdleState)
            curState = BossStates_Jan.idle;
        else if(stateMachine.currentState == bossChaseState)
            curState = BossStates_Jan.chase;
        else if (stateMachine.currentState == bossBasicAttackState)
            curState = BossStates_Jan.basicAttack;
        else if (stateMachine.currentState == BossPattern1State)
            curState = BossStates_Jan.pattern1;
        else if (stateMachine.currentState == BossPattern2State)
            curState = BossStates_Jan.pattern2;        
        else if (stateMachine.currentState == BossPattern3State)
            curState = BossStates_Jan.pattern3;
        else if (stateMachine.currentState == bossMonsterSpawnState)
            curState = BossStates_Jan.spawnMonster;
    }

    //피격
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            OnDamaged(1);
        }
    }

    public bool IsWithinChaseRange() //플레이어가 추적 범위 내에 있는지 check
    {
        float dist = Vector3.Magnitude(player.transform.position - this.transform.position);
        if (dist < chaseRange)
            return true;
        return false;
    }

    public bool IsWithinAttackRange() //플레이어가 공격 범위 내에 있는지 check
    {
        float dist = Vector3.Magnitude(player.transform.position - this.transform.position);
        if (dist < attackRange)
            return true;
        return false;
    }

    //데미지 처리
    public virtual void OnDamaged(int damage)
    {
        SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.Damage);

        curHP -= damage;
        //피격 시, 패시브 스킬
        //독사
        if (SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.DS14))
        {
            //지속 데미지 -> duration과 interval이 없음
            StartCoroutine(DOTDamage(5.0f, 1.0f, 1));
        }
        //구삥
        if (SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.GPP19))
        {
            //9% 확률로 몬스터 기절 duration
        }

        if (curHP <= 0)
        {
            Dead();
        }
    }

    IEnumerator DOTDamage(float duration, float interval, int perDamage)
    {
        float timer = interval;
        while (duration < 0.0f)
        {
            yield return null;
            timer -= Time.deltaTime;

            if (timer < 0.0f)
            {
                duration -= interval;
                timer = interval;
                curHP -= perDamage;
            }
        }
    }

    //죽음
    public virtual void Dead()
    {
        if (!isDead)
        {
            isDead = true;

            MonsterBase[] monsterBases = FindObjectsByType<MonsterBase>(FindObjectsSortMode.None); 
            Debug.Log("Dead!!!!!!!");

            for (int i = 0; i < monsterBases.Length; i++)
            {
                monsterBases[i].Dead();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, pattern2AttackRange);
    }
}
