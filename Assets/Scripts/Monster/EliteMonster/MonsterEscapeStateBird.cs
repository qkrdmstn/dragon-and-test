using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEscapeStateBird : MonsterState2
{
    private MonsterEliteBird monster;
    public MonsterEscapeStateBird(MonsterStateMachine2 _stateMachine, GameObject _player, MonsterEliteBird _monster) : base(_stateMachine, _player)
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

        Vector3 targetPosition = (monster.transform.position - player.transform.position).normalized * monster.chaseRange;
        Debug.DrawRay(monster.transform.position, targetPosition, Color.yellow);
        monster.agent.SetDestination(targetPosition);

        if (monster.distanceToPlayer > monster.attackRange) stateMachine.ChangeState(monster.chaseState);
        else if (monster.distanceToPlayer < monster.attackRange && monster.distanceToPlayer > monster.escapeRange) stateMachine.ChangeState(monster.attackState);
    }
}