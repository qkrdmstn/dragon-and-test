using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackStateBase : MonsterState
{
    protected Coroutine attackCoroutine;
    public MonsterAttackStateBase(MonsterStateMachine _stateMachine, Player _player, MonsterBase _monster) : base(_stateMachine, _player, _monster)
    {
        attackCoroutine = null;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
        if(attackCoroutine != null)
            monster.StopCoroutine(attackCoroutine);
    }

}
