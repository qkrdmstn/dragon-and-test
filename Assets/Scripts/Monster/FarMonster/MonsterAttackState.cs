using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackState : MonsterState
{
    private MonsterFar monster;
    public MonsterAttackState(MonsterStateMachine _stateMachine, GameObject _player, MonsterFar _monster) : base(_stateMachine, _player)
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

        //Attack
        monster.monsterShootTimer -= Time.deltaTime;

        if(!monster.isReloading && !monster.isKnockedBack) monster.Attack();

        if (monster.distanceToPlayer > monster.haltRange)
            stateMachine.ChangeState(monster.chaseAttackState);
    }
}
