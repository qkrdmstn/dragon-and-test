using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChaseState_Jan : BossState_Jan
{
    public BossChaseState_Jan(Boss_Jan _boss, BossStateMachine _stateMachine, Player _player) : base(_boss, _stateMachine, _player)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        //플레이어 추적
        Vector3 moveDir = player.transform.position - boss.transform.position;
        moveDir.Normalize();
        boss.SetVelocity(moveDir * boss.moveSpeed);

        //공격 범위 체크 및 상태 변경
        if (boss.IsWithinAttackRange())
            stateMachine.ChangeState(boss.bossBasicAttackState);
    }

    public override void Exit()
    {
        base.Exit();

        //속도 초기화
        boss.SetVelocity(Vector3.zero);
        //player.anim.SetBool(animBoolName, false);
    }
}
