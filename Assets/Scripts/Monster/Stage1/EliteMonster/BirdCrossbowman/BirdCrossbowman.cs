using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BirdCrossbowman : MonsterBase
{
    [Header("---------------BirdCrossbowman---------------")]
    [Header("Attack Info")]
    public int waveNum = 3;
    public int bulletNumPerWave = 10;
    public float waveInterval = 1.0f;
    public float attackRangeAngle = 65.0f;
    public float bulletSpeed = 6.0f;
    public float reloadDelay = 3.0f;

    [Header("Escape Info")]
    public float escapeDist = 15.0f;
    public float escapeSpeed = 5.0f;
    public float escapeAngleOffset = 75.0f;
    [Tooltip("Min(inclusive), Max(exclusive)")] public Vector2 escapeDirChangePeriod;

    #region Addtional States
    public MonsterAttackState_BirdCrossbowman attackState;
    public MonsterEscapeState_BirdCrossbowman escapeState;
    #endregion

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(spawnState);
    }

    public override void InitStates()
    {
        base.InitStates();

        spawnState = new MonsterSpawnStateBase(stateMachine, player, this);
        idleState = new MonsterIdleStateBase(stateMachine, player, this);
        deadState = new MonsterDeadStateBase(stateMachine, player, this);

        chaseState = new MonsterChaseState_BirdCrossbowman(stateMachine, player, this);
        attackState = new MonsterAttackState_BirdCrossbowman(stateMachine, player, this);
        escapeState = new MonsterEscapeState_BirdCrossbowman(stateMachine, player, this);
    }
}