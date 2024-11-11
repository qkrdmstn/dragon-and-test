using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseState_Jan : MonsterChaseStateBase
{
    private new Boss_Jan monster;
    public MonsterChaseState_Jan(MonsterStateMachine _stateMachine, Player _player, Boss_Jan _boss) : base(_stateMachine, _player, _boss)
    {
        monster = _boss;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        //공격 범위 체크 및 상태 변경
        if (monster.IsWithinAttackRange())
            stateMachine.ChangeState(monster.basicAttackState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
