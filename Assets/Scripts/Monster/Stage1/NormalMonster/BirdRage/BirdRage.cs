using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BirdRage : MonsterBase
{
    [Header("---------------BirdRage---------------")]
    [Header("Attack Info")]
    public float attackCoolTime = 1.0f;
    public float rageSpeed = 6.0f;
    public GameObject attackRangeObject;

    #region Addtional States
    public MonsterRageState_BirdRage rageState;
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

        chaseState = new MonsterChaseState_BirdRage(stateMachine, player, this);
        rageState = new MonsterRageState_BirdRage(stateMachine, player, this);
    }

    public override void OnDamaged(int damage)
    {
        if(stateMachine.currentState != rageState)
            stateMachine.ChangeState(rageState);
        base.OnDamaged(damage);
    }
}