using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdleStateBase : MonsterState
{
    public MonsterIdleStateBase(MonsterStateMachine _stateMachine, Player _player, MonsterBase _monster) : base(_stateMachine, _player, _monster)
    {
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
    }

}
