using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdleStateBird : MonsterState
{
    private MonsterEliteBird monster;
    public MonsterIdleStateBird(MonsterStateMachine _stateMachine, GameObject _player, MonsterEliteBird _monster) : base(_stateMachine, _player)
    {
        monster = _monster;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if(monster.distanceToPlayer < monster.chaseRange) stateMachine.ChangeState(monster.idleState);
    }
}
