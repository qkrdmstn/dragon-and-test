using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private Vector2 dashDir;
    private Vector2 dash;
    private float dashSpeed;

    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, PlayerAnimState _animStateName) : base(_player, _stateMachine, _animStateName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //Attack Disable Setting
        player.isAttackable = false;

        //Position Save
        player.PositionHistorySave();

        //Dash Setting
        dashDir = new Vector2(xInput, yInput);
        dashDir.Normalize();
        dashSpeed = player.ClacSpeed(player.dashSpeed);

        stateTimer = player.dashDuration;
        SoundManager.instance.SetEffectSound(SoundType.Player, PlayerSfx.Dash);
    }

    public override void Exit()
    {
        base.Exit();

        if (!player.isFall)
        {
            //Attack Able Setting
            player.isAttackable = true;
            Gun curGun = ItemManager.instance.gunController.GetCurGunComponent();
            curGun.shootTimer -= player.dashDuration;

            player.SetVelocity(0, 0);
        }
    }

    public override void Update()
    {
        base.Update();

        //Exponantial
        dash = dashDir * dashSpeed * Mathf.Exp(player.dashExpCoefficient * (player.dashDuration - stateTimer));
        player.SetVelocity(dash.x, dash.y);
        player.animController.SetAnim(PlayerAnimState.Wave, xInput, yInput);
        //Dash Duration
        if (stateTimer < 0.0)
            stateMachine.ChangeState(player.idleState);
    }
}
