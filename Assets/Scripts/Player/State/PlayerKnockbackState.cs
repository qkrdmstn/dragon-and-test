using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockbackState : PlayerState
{
    private Vector2 knockbackVel;

    public PlayerKnockbackState(Player _player, PlayerStateMachine _stateMachine, PlayerAnimState _animStateName) : base(_player, _stateMachine, _animStateName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //Attack Disable Setting
        player.isAttackable = false;

        //Position Save
        player.PositionHistorySave();

        stateTimer = 0;
        knockbackVel = player.knockbackDir * player.knockbackMagnitude;
        player.SetVelocity(knockbackVel);

    }

    public override void Exit()
    {
        base.Exit();

        //Attack Disable Setting
        player.isAttackable = true;

        player.SetVelocity(0, 0);
    }

    public override void Update()
    {
        base.Update();

        //Exponantial
        knockbackVel = knockbackVel * Mathf.Exp(player.knockbackExpCoefficient * (- stateTimer));
        player.SetVelocity(knockbackVel);
        if (player.rb.velocity.magnitude < 0.1)
            stateMachine.ChangeState(player.idleState);
    }
}
