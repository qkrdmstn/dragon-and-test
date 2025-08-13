using System.Collections;
using UnityEngine;

public class Boss_Jan : Boss
{
    public enum BossStates_Jan
    {
        idle,
        chase,
        basicAttack,
        pattern1,
        pattern2,
        pattern3,
        pattern4,
        spawnMonster,
        hit
    }

    [Header("---------------Boss_Jan---------------")]
    //Temp
    public BossStates_Jan initState;
    public BossStates_Jan curState;

    [Header("Basic Attack State Info")]
    public GameObject bulletPrefab;
    public float shootDelay;
    public float reloadTime;
    public int magazineSize;
    public int loadedBullet;
    public float maxRecoilDegree;
    public float bulletSpeed;
    public float bulletRange;
    public int reloadCnt = 0;

    [Header("Pattern1 Info")]
    public float pattern1Prob = 0.35f;
    public float sphereShootNum;
    public float sphereInterval;
    public float spherePathInterval;
    public float sphereBulletSpeed = 7.0f;
    public float sphereBulletRange;

    public float pathInterval;
    public float pathBulletLifeTime;
    public float pathBulletSpeed = 3.5f;
    public float pathBulletRange;

    public float waveInterval = 2.0f;
    public float waveNum;

    [Header("Pattern2 Info")]
    public float pattern2Prob = 0.25f;
    public GameObject displayPrefab;
    public float pattern2Delay;
    public int pattern2ShootNum;
    public float pattern2ShootDelay;
    public float pattern2RangeDisplayTime;
    public int pattern2ShootOffset;
    public float pattern2AttackRange;
    public bool isPattern2;

    [Header("Pattern3 Info")]
    public float pattern3Prob = 0.15f;
    public Pattern3Object pattern3Object;
    public float pattern3Delay = 1.0f;
    public float pattern3RotationTime = 1.0f;
    public float pattern3DisplayTime = 1.0f;

    [Header("Pattern4 Info")]
    public float pattern4Prob = 0.25f;
    public float pattern4StartInterval = 1.1f;
    public float pattern4EndInterval = 0.5f;
    public float pattern4BulletSpeed = 0.6f;
    public float pattern4BulletSpreadDelay = 2.0f;
    public float pattern4BulletMoveDelay = 2.0f;

    [Header("Spawn Monster State Info")]
    public GameObject[] spawnMosnterPrefabs;
    public BlockInfo bossField;
    private Vector2Int maxGridPos;
    public float spawnPeriod;
    public float spawnWaveCnt = 0;
    public float spawnDelay = 1.0f;
    public float spawnTimer;

    #region States
    public MonsterBasicAttackState_Jan basicAttackState;
    public MonsterPattern1State_Jan pattern1State;
    public MonsterPattern2State_Jan pattern2State;
    public MonsterPattern3State_Jan pattern3State;
    public MonsterPattern4State_Jan pattern4State;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        spawnTimer = spawnPeriod;
    }

    protected override void Start()
    {
        switch (initState)
        {
            case BossStates_Jan.idle:
                stateMachine.Initialize(idleState);
                break;
            case BossStates_Jan.chase:
                stateMachine.Initialize(chaseState);
                break;
            case BossStates_Jan.basicAttack:
                stateMachine.Initialize(basicAttackState);
                break;
            case BossStates_Jan.pattern1:
                stateMachine.Initialize(pattern1State);
                break;
            case BossStates_Jan.pattern2:
                stateMachine.Initialize(pattern2State);
                break;
            case BossStates_Jan.pattern3:
                stateMachine.Initialize(pattern3State);
                break;
            case BossStates_Jan.pattern4:
                stateMachine.Initialize(pattern4State);
                break;
            default:
                stateMachine.Initialize(idleState);
                break;
        }
    }


    public override void InitComponents()
    {
        base.InitComponents();
        bossField = FindObjectOfType<BlockInfo>();
        bossField.InitializeBlockInfo(0);
        maxGridPos = bossField.GetMaxGridPos();

        pattern3Object = FindObjectOfType<Pattern3Object>();
    }

    public override void InitStates()
    {
        stateMachine = new MonsterStateMachine(this);
        isStateChangeable = false;

        idleState = new MonsterIdleStateBase(stateMachine, player, this);
        deadState = new MonsterDeadState_Jan(stateMachine, player, this);

        chaseState = new MonsterChaseState_Jan(stateMachine, player, this);
        basicAttackState = new MonsterBasicAttackState_Jan(stateMachine, player, this);
        pattern1State = new MonsterPattern1State_Jan(stateMachine, player, this);
        pattern2State = new MonsterPattern2State_Jan(stateMachine, player, this);
        pattern3State = new MonsterPattern3State_Jan(stateMachine, player, this);
        pattern4State = new MonsterPattern4State_Jan(stateMachine, player, this);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (stateMachine.currentState != idleState)
            spawnTimer -= Time.deltaTime;

        if (!isDead && spawnTimer < 0.0f)
            SpawnMonster();

        if (stateMachine.currentState == idleState)
            curState = BossStates_Jan.idle;
        else if (stateMachine.currentState == chaseState)
            curState = BossStates_Jan.chase;
        else if (stateMachine.currentState == basicAttackState)
            curState = BossStates_Jan.basicAttack;
        else if (stateMachine.currentState == pattern1State)
            curState = BossStates_Jan.pattern1;
        else if (stateMachine.currentState == pattern2State)
            curState = BossStates_Jan.pattern2;
        else if (stateMachine.currentState == pattern3State)
            curState = BossStates_Jan.pattern3;
        else if (stateMachine.currentState == pattern4State)
            curState = BossStates_Jan.pattern4;
    }

    public bool IsWithinChaseRange() //플레이어가 추적 범위 내에 있는지 check
    {
        float dist = Vector3.Magnitude(player.transform.position - this.transform.position);
        if (dist < chaseDist)
            return true;
        return false;
    }

    public bool IsWithinAttackRange() //플레이어가 공격 범위 내에 있는지 check
    {
        float dist = Vector3.Magnitude(player.transform.position - this.transform.position);
        if (dist < attackDist)
            return true;
        return false;
    }

    public override void Dead()
    {
        if (!isDead)
            MoneyDrop();
        base.Dead();
    }

    private void SpawnMonster()
    {
        spawnTimer = spawnPeriod;

        int spawnNum = 3;
        //if (boss.spawnWaveCnt == 0)
        //    spawnNum = 4;
        //else if (boss.spawnWaveCnt == 1)
        //    spawnNum = 5;

        Vector2Int[] spawnGridPos = GetSpawnGridPos(spawnNum);

        //몬스터 스폰
        for (int i = 0; i < spawnNum; i++)
        {
            int idx = Random.Range(0, spawnMosnterPrefabs.Length);
            GameObject.Instantiate(spawnMosnterPrefabs[idx], bossField.GridToWorldPosition(spawnGridPos[i]), Quaternion.identity);
        }
    }

    private Vector2Int[] GetSpawnGridPos(int spawnNum)
    {
        Vector2Int[] spawnGridPos = new Vector2Int[spawnNum];

        int i = 0;
        int cnt = 0;
        while (cnt < spawnNum)
        {
            int gridPosX = Random.Range(0, maxGridPos.x);
            int gridPosY = Random.Range(0, maxGridPos.y);

            for (i = 0; i < cnt; i++)
            {
                if (spawnGridPos[i].x == gridPosX && spawnGridPos[i].y == gridPosY)
                    break;
            }
            if (i == cnt)
            {
                spawnGridPos[cnt] = new Vector2Int(gridPosX, gridPosY);
                cnt++;
            }
        }

        return spawnGridPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, chaseDist);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, attackDist);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, pattern2AttackRange);
    }

}
