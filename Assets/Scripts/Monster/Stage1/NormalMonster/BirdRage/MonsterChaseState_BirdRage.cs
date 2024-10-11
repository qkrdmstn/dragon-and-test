using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterChaseState_BirdRage : MonsterChaseStateBase
{
    protected new BirdRage monster;

    public MonsterChaseState_BirdRage(MonsterStateMachine _stateMachine, Player _player, BirdRage _monster) : base(_stateMachine, _player, _monster)
    {
        monster = _monster;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        //공격 범위 내에 Player가 존재한다면, 공격 상태로 변경
        float dist =  Vector3.Distance(monster.transform.position, player.transform.position);
        if (dist < monster.attackDist)
            stateMachine.ChangeState(monster.rageState);
        else if (dist > monster.chaseDist)
            stateMachine.ChangeState(monster.idleState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
