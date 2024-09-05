using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdleState_Jan : BossState_Jan
{
    public BossIdleState_Jan(Boss_Jan _boss, BossStateMachine _stateMachine, Player _player) : base(_boss, _stateMachine, _player)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }


    public override void Update()
    {
        base.Update();

        if (boss.isDead)
        {
            boss.SetVelocity(Vector2.zero);
            return;
        }

        //추적 상태 확인
        if (boss.IsWithinChaseRange())
            stateMachine.ChangeState(boss.bossChaseState);
    }

    public override void Exit()
    {
        base.Exit();
        //player.anim.SetBool(animBoolName, false);
    }
}
