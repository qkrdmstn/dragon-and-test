using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseStateMage : MonsterState2
{
    private Vector2 direction;
    private MonsterMage monster;
    public MonsterChaseStateMage(MonsterStateMachine2 _stateMachine, GameObject _player, MonsterMage _monster) : base(_stateMachine, _player)
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

        monster.agent.SetDestination(player.transform.position);
        
        //if(monster.distanceToPlayer>1f && !monster.isKnockedBack) monster.rigidBody.velocity = direction * monster.tempSpeed;
        
        if (monster.distanceToPlayer < monster.attackRange && monster.tempcool<=0.0)
            stateMachine.ChangeState(monster.attackState);
    }
}
