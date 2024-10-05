using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BirdWarrior1 : MonsterBase
{
    [Header("---------------MonsterNear---------------")]
    [Header("Attack Info")]
    public int shootNum = 3;
    public float shootDelay = 0.7f;
    public float attackRange = 3.0f;
    public GameObject swordAura;


    #region States
    public MonsterChaseState_BirdWarrior1 chaseState;
    public MonsterIdleState_BirdWarrior1 idleState;
    public MonsterAttackState_BirdWarrior1 attackState;
    #endregion


    public override void InitStates()
    {
        base.InitStates();

        idleState = new MonsterIdleState_BirdWarrior1(stateMachine, player, this);
        chaseState = new MonsterChaseState_BirdWarrior1(stateMachine, player, this);
        attackState = new MonsterAttackState_BirdWarrior1(stateMachine, player, this);

        stateMachine.Initialize(chaseState);
    }
}