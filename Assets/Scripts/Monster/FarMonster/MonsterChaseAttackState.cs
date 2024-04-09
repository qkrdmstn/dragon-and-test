using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseAttackState : MonsterState
{
    private Vector2 direction;
    private MonsterFar monster;
    public MonsterChaseAttackState(MonsterStateMachine _stateMachine, GameObject _player, MonsterFar _monster) : base(_stateMachine, _player)
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

        //Chase
        direction = monster.chase.tempDir;
        monster.transform.Translate(direction * monster.moveSpeed * Time.deltaTime);

        //Attack
        monster.monsterShootTimer -= Time.deltaTime;

        if(!monster.isReloading) monster.Attack();

        if (monster.distanceToPlayer < monster.haltRange)
            stateMachine.ChangeState(monster.attackState);
        
        if (monster.distanceToPlayer > monster.attackRange)
            stateMachine.ChangeState(monster.chaseState);
    }

    
}
