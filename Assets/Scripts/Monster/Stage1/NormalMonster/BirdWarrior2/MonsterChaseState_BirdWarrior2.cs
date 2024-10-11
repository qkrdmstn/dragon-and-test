using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterChaseState_BirdWarrior2 : MonsterChaseStateBase
{
    protected new BirdWarrior2 monster;

    public MonsterChaseState_BirdWarrior2(MonsterStateMachine _stateMachine, Player _player, BirdWarrior2 _monster) : base(_stateMachine, _player, _monster)
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
            stateMachine.ChangeState(monster.attackState);
        else if(dist > monster.chaseDist)
            stateMachine.ChangeState(monster.idleState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
