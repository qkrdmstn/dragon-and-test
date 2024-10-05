using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdleState_BirdWarrior1 : MonsterIdleStateBase
{
    protected BirdWarrior1 monster;

    public MonsterIdleState_BirdWarrior1(MonsterStateMachine _stateMachine, Player _player, BirdWarrior1 _monster) : base(_stateMachine, _player, _monster)
    {
        this.monster = _monster;
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
