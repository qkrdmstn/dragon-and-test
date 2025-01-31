using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterChaseState_BirdTanker : MonsterChaseStateBase
{
    protected new BirdTanker monster;

    public MonsterChaseState_BirdTanker(MonsterStateMachine _stateMachine, Player _player, BirdTanker _monster) : base(_stateMachine, _player, _monster)
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

        float dist = Vector3.Distance(monster.transform.position, player.transform.position);
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
