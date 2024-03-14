using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackState : MonsterState
{
    public MonsterAttackState(Monster _monster, MonsterStateMachine _stateMachine, GameObject _player) : base(_monster, _stateMachine, _player)
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

        //Attack
        monster.monsterShootTimer -= Time.deltaTime;

        if(!monster.isReloading){
            if (monster.loadedBullet > 0) monster.Shoot();
            else if (monster.loadedBullet <= 0 ) monster.Reload();
        }

        if (distanceToPlayer > monster.haltRange)
            stateMachine.ChangeState(monster.chaseAttackState);
    }
}
