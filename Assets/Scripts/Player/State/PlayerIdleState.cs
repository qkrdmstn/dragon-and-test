using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, AnimState _animStateName) : base(_player, _stateMachine, _animStateName)
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

        player.animController.SetAnim(AnimState.Idle, mouseDir.x, mouseDir.y);
        

        if (xInput != 0 || yInput != 0)
            stateMachine.ChangeState(player.moveState);
    }
}
