using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseState : MonsterState
{
    private Vector2 direction;
    private MonsterFar monster;
    public MonsterChaseState(MonsterStateMachine _stateMachine, GameObject _player, MonsterFar _monster) : base(_stateMachine, _player)
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

        direction = monster.chase.tempDir;
        if(!monster.isKnockedBack) monster.rigidBody.velocity = direction * monster.moveSpeed;
        
        if (monster.distanceToPlayer < monster.attackRange)
            stateMachine.ChangeState(monster.chaseAttackState);
    }
}
