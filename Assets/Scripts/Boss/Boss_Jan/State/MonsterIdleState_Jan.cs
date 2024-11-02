using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdleState_Jan : MonsterIdleStateBase
{
    private new Boss_Jan monster;
    public MonsterIdleState_Jan(MonsterStateMachine _stateMachine, Player _player, Boss_Jan _boss) : base(_stateMachine, _player, _boss)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }


    public override void Update()
    {
        base.Update();

        //추적 상태 확인
        if (monster.IsWithinChaseRange())
            stateMachine.ChangeState(monster.chaseState);
    }

    public override void Exit()
    {
        base.Exit();
        //player.anim.SetBool(animBoolName, false);
    }
}
