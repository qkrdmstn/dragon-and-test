using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseState : MonsterState
{
    private Vector2 direction;
    public MonsterChaseState(Monster _monster, MonsterStateMachine _stateMachine, GameObject _player) : base(_monster, _stateMachine, _player)
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

        direction = (player.transform.position - monster.transform.position).normalized;
        monster.transform.Translate(direction * monster.moveSpeed * Time.deltaTime);
        
        if (distanceToPlayer < monster.attackRange)
            stateMachine.ChangeState(monster.chaseAttackState);
    }
}
