using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdleStateDash : MonsterState
{
    private MonsterDash monster;
    public MonsterIdleStateDash(MonsterStateMachine _stateMachine, GameObject _player, MonsterDash _monster) : base(_stateMachine, _player)
    {
        monster = _monster;
    }

    public override void Enter()
    {
        base.Enter();
        //monster.SpeedToZero();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (monster.distanceToPlayer < monster.chaseRange && monster.tempcool <= 0.0f) stateMachine.ChangeState(monster.chaseState);
        
    }
}
