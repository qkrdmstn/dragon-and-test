using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private Vector2 dashDir;
    private Vector2 dash;

    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //To do. 公利 眠啊

        //Dash Direction Setting
        dashDir = new Vector2(xInput, yInput);
        dashDir.Normalize();

        stateTimer = player.dashDuration;
    }

    public override void Exit()
    {
        base.Exit();

        //To do. 公利 秦力 眠啊


        player.SetVelocity(0, 0);
    }

    public override void Update()
    {
        base.Update();

        dash = dashDir * player.dashSpeed;
        player.SetVelocity(dash.x, dash.y);

        //Dash Duration
        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);
    }
}
