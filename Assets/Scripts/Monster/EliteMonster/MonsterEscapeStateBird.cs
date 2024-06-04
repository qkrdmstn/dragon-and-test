using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEscapeStateBird : MonsterState
{
    private MonsterEliteBird monster;
    public MonsterEscapeStateBird(MonsterStateMachine _stateMachine, GameObject _player, MonsterEliteBird _monster) : base(_stateMachine, _player)
    {
        monster = _monster;
    }


    public override void Enter()
    {
        base.Enter();
        monster.SpeedReturn();

        Vector3 targetPosition = monster.transform.position - player.transform.position.normalized * monster.chaseRange * 1.2f;
        monster.agent.SetDestination(targetPosition);

    }

    public override void Exit()
    {
        base.Exit();
        monster.SpeedToZero();
    }

    public override void Update()
    {
        base.Update();

        if (monster.distanceToPlayer > monster.attackRange * 0.9f) stateMachine.ChangeState(monster.attackState);
        else stateMachine.ChangeState(monster.escapeState);

    }
}