using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern3State_Jan : BossState_Jan
{
    public BossPattern3State_Jan(Boss_Jan _boss, BossStateMachine _stateMachine, Player _player) : base(_boss, _stateMachine, _player)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateMachine.ChangeState(boss.BossPattern2State);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }
}
