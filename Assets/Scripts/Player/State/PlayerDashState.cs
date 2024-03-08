using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private Vector2 dashDir;
    private Vector2 dash;

    private bool attackFlag;
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //Attack Disable Setting
        player.isAttackable = false;

        //To do. 무적 추가, 전투 가능 지역 판단..

        //Dash Direction Setting
        dashDir = new Vector2(xInput, yInput);
        dashDir.Normalize();

        stateTimer = player.dashDuration;
    }

    public override void Exit()
    {
        base.Exit();

        //Attack Able Setting
        player.isAttackable = true;
        player.gun.shootTimer -= player.dashDuration;

        //To do. 무적 해제 추가

        player.SetVelocity(0, 0);
    }

    public override void Update()
    {
        base.Update();

        dash = dashDir * player.dashSpeed;
        player.SetVelocity(dash.x, dash.y);

        //Dash Duration
        if (stateTimer < 0.0)
            stateMachine.ChangeState(player.idleState);
    }
}
