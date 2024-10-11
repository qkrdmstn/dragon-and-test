using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BirdWarrior1 : MonsterBase
{
    [Header("---------------BirdWarrior1---------------")]
    [Header("Attack Info")]
    public int shootNum = 3;
    public float shootDelay = 0.7f;
    public GameObject swordAura;

    #region Addtional States
    public MonsterAttackState_BirdWarrior1 attackState;
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

        chaseState = new MonsterChaseState_BirdWarrior1(stateMachine, player, this);
        attackState = new MonsterAttackState_BirdWarrior1(stateMachine, player, this);
    }
}