using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterChaseState_BirdWarrior1 : MonsterChaseStateBase
{
    protected new BirdWarrior1 monster;

    public MonsterChaseState_BirdWarrior1(MonsterStateMachine _stateMachine, Player _player, BirdWarrior1 _monster) : base(_stateMachine, _player, _monster)
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
        float dist =  Vector3.Distance(monster.transform.position, player.transform.position);
        if (dist < monster.attackDist)
            stateMachine.ChangeState(monster.attackState);
        else if (dist > monster.chaseDist)
            stateMachine.ChangeState(monster.idleState);
    }

    public override void Exit()
    {
        base.Exit();
    }


}
