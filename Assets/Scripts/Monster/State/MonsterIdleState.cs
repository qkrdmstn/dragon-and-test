using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdleState : MonsterState
{
    public MonsterIdleState(Monster _monster, MonsterStateMachine _stateMachine, GameObject _player) : base(_monster, _stateMachine, _player)
    {
    }

    public override void Enter()
    {
        monster.loadedBullet = monster.magazineSize;
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        /*
        if (distanceToPlayer < monster.recognitionRange)
            stateMachine.ChangeState(monster.chaseState);
        */

        stateMachine.ChangeState(monster.chaseState);
    }
}
