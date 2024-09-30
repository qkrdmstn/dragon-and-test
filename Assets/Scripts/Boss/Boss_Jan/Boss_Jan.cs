using System.Collections;
using UnityEngine;

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

public class Boss_Jan : Boss
{
    //Temp
    public BossStates_Jan initState;
    public BossStates_Jan curState;

    [Header("Chase State Info")]
    public float chaseRange;

    [Header("Basic Attack State Info")]
    public GameObject bulletPrefab;
    public float attackRange;
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

    [Header("Spawn Monster State Info")]
    public GameObject[] spawnMosnterPrefabs;
    public BlockInfo bossField;
    public float spawnPeriod;
    public float spawnWaveCnt = 0;
    public float spawnDelay = 1.0f;
    public float spawnTimer;

    [Header("Drop Item Info")]
    public GameObject moneyPrefab;
    public int moneyValue;

    #region States
    public BossStateMachine stateMachine;

    public BossIdleState_Jan bossIdleState;
    public BossChaseState_Jan bossChaseState;
    public BossBasicAttackState_Jan bossBasicAttackState;
    public BossPattern1State_Jan bossPattern1State;
    public BossPattern2State_Jan bossPattern2State;
    public BossPattern3State_Jan bossPattern3State;
    public BossPattern4State_Jan bossPattern4State;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        spawnTimer = spawnPeriod;
        InitComponent();

        InitState();
    }

    private void InitComponent()
    {
        bossField = FindObjectOfType<BlockInfo>();
        bossField.InitializeBlockInfo(0);
        pattern3Object = FindObjectOfType<Pattern3Object>();

        moneyPrefab = Resources.Load<GameObject>("Prefabs/Item/Money");
    }

    private void InitState()
    {
        stateMachine = new BossStateMachine();

        bossIdleState = new BossIdleState_Jan(this, stateMachine, player);
        bossChaseState = new BossChaseState_Jan(this, stateMachine, player);
        bossBasicAttackState = new BossBasicAttackState_Jan(this, stateMachine, player);
        bossPattern1State = new BossPattern1State_Jan(this, stateMachine, player);
        bossPattern2State = new BossPattern2State_Jan(this, stateMachine, player);
        bossPattern3State = new BossPattern3State_Jan(this, stateMachine, player);
        bossPattern4State = new BossPattern4State_Jan(this, stateMachine, player);

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
                stateMachine.Initialize(bossPattern1State);
                break;
            case BossStates_Jan.pattern2:
                stateMachine.Initialize(bossPattern2State);
                break;
            case BossStates_Jan.pattern3:
                stateMachine.Initialize(bossPattern3State);
                break;
            case BossStates_Jan.pattern4:
                stateMachine.Initialize(bossPattern4State);
                break;
            default:
                stateMachine.Initialize(bossIdleState);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.currentState.Update();

        if (stateMachine.currentState == bossIdleState)
            curState = BossStates_Jan.idle;
        else if (stateMachine.currentState == bossChaseState)
            curState = BossStates_Jan.chase;
        else if (stateMachine.currentState == bossBasicAttackState)
            curState = BossStates_Jan.basicAttack;
        else if (stateMachine.currentState == bossPattern1State)
            curState = BossStates_Jan.pattern1;
        else if (stateMachine.currentState == bossPattern2State)
            curState = BossStates_Jan.pattern2;
        else if (stateMachine.currentState == bossPattern3State)
            curState = BossStates_Jan.pattern3;
        else if (stateMachine.currentState == bossPattern4State)
            curState = BossStates_Jan.pattern4;
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

    public override void Dead()
    {
        if (!isDead)
            MoneyDrop();
        base.Dead();
    }

    private void MoneyDrop()
    {
        ItemObject item = moneyPrefab.GetComponent<ItemObject>();
        GameObject moneyObj = Instantiate(moneyPrefab, this.transform.position + Vector3.down * 3, Quaternion.identity);
        moneyObj.GetComponent<InteractionData>().sequence = moneyValue;
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
