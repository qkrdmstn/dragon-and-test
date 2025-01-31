using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdleState_TutorialArcher1 : MonsterIdleStateBase
{
    protected new TutorialBirdArcher1 monster;
    public MonsterIdleState_TutorialArcher1(MonsterStateMachine _stateMachine, Player _player, TutorialBirdArcher1 _monster) : base(_stateMachine, _player, _monster)
    {
        monster = _monster;
    }

    public override void Enter()
    {
        base.Enter();
        monster.SetSpeed(0.0f);
    }

    public override void Update()
    {
        base.Update();

        if (monster.isAttackable)
            stateMachine.ChangeState(monster.chaseState);
    }

    public override void Exit()
    {
        base.Exit();
    }

}
