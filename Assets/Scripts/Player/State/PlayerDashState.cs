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

        // 措矫 吝 公利
        player.gameObject.layer = 7;
        player.spriteRenderer.color = new Color(1, 0, 0);

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

        // 公利 秦力
        player.gameObject.layer = 6;
        player.spriteRenderer.color = new Color(1, 1, 1);

        player.SetVelocity(0, 0);
    }

    public override void Update()
    {
        base.Update();

        //Constant
        switch (player.dashMode)
        {
            case 0:
                //Constant
                if (player.dashDuration * 0.33 <= stateTimer)
                    dash = dashDir * player.dashSpeed;
                else
                    dash = dashDir * player.dashSpeed * 0.3f;
                break;
            case 1:
                //Constant & Exponantial
                if (player.dashDuration * 0.5f <= stateTimer)
                    dash = dashDir * player.dashSpeed;
                else
                    dash = dashDir * player.dashSpeed * Mathf.Exp(player.expCoefficient * (player.dashDuration * 0.67f - stateTimer));
                break;
            case 2:
                //Exponantial
                dash = dashDir * player.dashSpeed * Mathf.Exp(player.expCoefficient * (player.dashDuration - stateTimer));
                break;
            case 3:
                //Cosine
                dash = dashDir * player.dashSpeed * Mathf.Cos(3.7f * (player.dashDuration - stateTimer));
                break;
            default:
                break;
        }

        player.SetVelocity(dash.x, dash.y);

        //Dash Duration
        if (stateTimer < 0.0)
            stateMachine.ChangeState(player.idleState);
    }
}
