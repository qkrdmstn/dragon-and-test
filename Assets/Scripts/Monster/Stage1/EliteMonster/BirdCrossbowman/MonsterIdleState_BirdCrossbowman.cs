using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterIdleState_BirdCrossbowman : MonsterIdleStateBase
{
    protected new BirdCrossbowman monster;

    public MonsterIdleState_BirdCrossbowman(MonsterStateMachine _stateMachine, Player _player, BirdCrossbowman _monster) : base(_stateMachine, _player, _monster)
    {
        monster = _monster;
    }

    public override void Enter()
    {
        base.Enter();
        monster.SetSpeed(0.0f);
        monster.monsterAnimController.SetAnim(CrossbowmanAnimState.Idle2, monster.CheckDir());
    }

    public override void Update()
    {
        float dist = Vector3.Distance(monster.transform.position, player.transform.position);
        if (dist < monster.escapeDist)
            stateMachine.ChangeState(monster.escapeState);
        else if(dist < monster.chaseDist)
            stateMachine.ChangeState(monster.chaseState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
