using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdleStateMage : MonsterState
{
    private MonsterMage monster;
    public MonsterIdleStateMage(MonsterStateMachine _stateMachine, GameObject _player, MonsterMage _monster) : base(_stateMachine, _player)
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
