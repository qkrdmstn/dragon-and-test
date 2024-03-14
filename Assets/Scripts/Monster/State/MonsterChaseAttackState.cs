using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseAttackState : MonsterState
{
    private Vector2 direction;
    public MonsterChaseAttackState(Monster _monster, MonsterStateMachine _stateMachine, GameObject _player) : base(_monster, _stateMachine, _player)
    {
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
        direction = (player.transform.position - monster.transform.position).normalized;
        monster.transform.Translate(direction * monster.moveSpeed * Time.deltaTime);

        //Attack
        monster.monsterShootTimer -= Time.deltaTime;

        if(!monster.isReloading){
            if (monster.loadedBullet > 0) monster.Shoot();
            else if (monster.loadedBullet <= 0 ) monster.Reload();
        }

        if (distanceToPlayer < monster.haltRange)
            stateMachine.ChangeState(monster.attackState);
        
        if (distanceToPlayer > monster.attackRange)
            stateMachine.ChangeState(monster.chaseState);
    }

    
}
