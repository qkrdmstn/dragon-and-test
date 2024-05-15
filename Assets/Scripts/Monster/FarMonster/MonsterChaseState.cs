using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseState : MonsterState
{
    private MonsterFar monster;
    public MonsterChaseState(MonsterStateMachine _stateMachine, GameObject _player, MonsterFar _monster) : base(_stateMachine, _player)
    {
        monster = _monster;
    }

    public override void Enter()
    {
        base.Enter();
        monster.SpeedReturn();
    }

    public override void Exit()
    {
        base.Exit();
        monster.SpeedToZero();
    }

    public override void Update()
    {
        base.Update();

        //navigate
        monster.agent.SetDestination(player.transform.position);

        if(monster.distanceToPlayer < monster.attackRange) stateMachine.ChangeState(monster.attackState);
        
    }
}
