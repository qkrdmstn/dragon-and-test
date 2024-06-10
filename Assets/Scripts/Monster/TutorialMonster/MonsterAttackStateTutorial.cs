using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackStateTutorial : MonsterState
{
    private MonsterTutorial monster;
    public MonsterAttackStateTutorial(MonsterStateMachine _stateMachine, GameObject _player, MonsterTutorial _monster) : base(_stateMachine, _player)
    {
        monster = _monster;
    }

    public override void Enter()
    {
        base.Enter();
        //monster.SpeedToZero();
    }

    public override void Exit()
    {
        base.Exit();
        //monster.SpeedReturn();
    }

    public override void Update()
    {
        base.Update();

        //Attack
        monster.Attack();

        if (monster.distanceToPlayer > monster.attackRange)
            stateMachine.ChangeState(monster.chaseState);
    }
}
