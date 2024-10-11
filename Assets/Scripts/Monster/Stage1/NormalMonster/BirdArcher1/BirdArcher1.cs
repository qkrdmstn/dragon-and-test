using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BirdArcher1 : MonsterBase
{
    [Header("---------------BirdArcher1---------------")]
    [Header("Attack Info")]
    public int shootNum = 4;
    public float shootDelay = 0.3f;
    public float shootOffsetAngle = 15.0f;
    public float reloadDelay = 2f;
    public float bulletSpeed = 9.0f;

    #region Addtional States
    public MonsterAttackState_BirdArcher1 attackState;
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

        chaseState = new MonsterChaseState_BirdArcher1(stateMachine, player, this);
        attackState = new MonsterAttackState_BirdArcher1(stateMachine, player, this);
    }
}