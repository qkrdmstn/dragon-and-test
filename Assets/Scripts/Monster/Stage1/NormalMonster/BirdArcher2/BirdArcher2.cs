using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BirdArcher2 : MonsterBase
{
    [Header("---------------BirdArcher2---------------")]
    [Header("Attack Info")]
    public int shootNum = 6;
    public float shootAngleOffset = 15.0f;
    public float reloadDelay = 2f;
    [Tooltip("Min(inclusive), Max(exclusive)")] public Vector2 bulletSpeedRange = new Vector2(3.0f, 7.0f);

    #region Addtional States
    public MonsterAttackState_BirdArcher2 attackState;
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

        chaseState = new MonsterChaseState_BirdArcher2(stateMachine, player, this);
        attackState = new MonsterAttackState_BirdArcher2(stateMachine, player, this);
    }
}