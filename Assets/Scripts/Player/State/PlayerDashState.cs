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

        //Attack Disable Setting
        player.isAttackable = false;

        // 대시 중 무적
        player.ChangeOnDamagedLayer();

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
        Gun curGun = GunManager.instance.currentGun.GetComponent<Gun>();
        curGun.shootTimer -= player.dashDuration;

        // 무적 해제
        player.ChangePlayerLayer();
        player.SetVelocity(0, 0);
    }

    public override void Update()
    {
        base.Update();

        //Exponantial
        dash = dashDir * player.dashSpeed * Mathf.Exp(player.dashExpCoefficient * (player.dashDuration - stateTimer));
        player.SetVelocity(dash.x, dash.y);

        //Dash Duration
        if (stateTimer < 0.0)
            stateMachine.ChangeState(player.idleState);
    }
}
