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
        Vector3 targetPosition = monster.transform.position - player.transform.position.normalized * monster.chaseRange * 1.2f;
        monster.agent.SetDestination(targetPosition);

        if(monster.distanceToPlayer > 0.6*monster.chaseRange) stateMachine.ChangeState(monster.attackState);
        
    }
}
