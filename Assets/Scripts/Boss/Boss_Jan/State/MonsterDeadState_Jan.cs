using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDeadState_Jan : MonsterDeadStateBase
{
    private new Boss_Jan monster;
    private BlockInfo bossField;
    private Vector2Int maxGridPos;

    public MonsterDeadState_Jan(MonsterStateMachine _stateMachine, Player _player, Boss_Jan _boss) : base(_stateMachine, _player, _boss)
    {
        monster = _boss;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

    }

    public override void Exit()
    {
        base.Exit();
        //player.anim.SetBool(animBoolName, false);
    }
}
