using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdleStateBase : MonsterState
{
    public MonsterIdleStateBase(MonsterStateMachine _stateMachine, Player _player, MonsterBase _monster) : base(_stateMachine, _player, _monster)
    {
    }

    public override void Enter()
    {
        base.Enter();
        monster.SetSpeed(0.0f);
    }

    public override void Update()
    {
        base.Update();

        float dist = Vector3.Distance(monster.transform.position, player.transform.position);
        if (dist < monster.chaseDist)
            stateMachine.ChangeState(monster.chaseState);
    }

    public override void Exit()
    {
        base.Exit();
    }

}
