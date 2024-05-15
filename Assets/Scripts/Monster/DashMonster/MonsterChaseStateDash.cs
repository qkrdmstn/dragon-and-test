using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseStateDash : MonsterState
{
    private Vector2 direction;
    private MonsterDash monster;
    public MonsterChaseStateDash(MonsterStateMachine _stateMachine, GameObject _player, MonsterDash _monster) : base(_stateMachine, _player)
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

        //if(monster.distanceToPlayer>1f && !monster.isKnockedBack) monster.rigidBody.velocity = direction * monster.tempSpeed;
        
        if (monster.distanceToPlayer < monster.attackRange && monster.tempcool<=0.0)
            stateMachine.ChangeState(monster.attackState);

        if (monster.distanceToPlayer > monster.chaseRange) stateMachine.ChangeState(monster.idleState);
    }
}
