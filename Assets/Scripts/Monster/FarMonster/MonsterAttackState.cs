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
        monster.SpeedToZero();
    }

    public override void Exit()
    {
        base.Exit();
        monster.SpeedReturn();
    }

    public override void Update()
    {
        base.Update();

        //Attack
        monster.Attack();

        if(monster.distanceToPlayer > monster.attackRange) stateMachine.ChangeState(monster.chaseState);
    }
}
