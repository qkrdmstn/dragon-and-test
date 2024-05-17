using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockbackState : PlayerState
{
    private Vector2 knockbackVel;

    public PlayerKnockbackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //Attack Disable Setting
        player.isAttackable = false;
        Debug.Log("enter");

        stateTimer = 0;
        knockbackVel = player.knockbackDir * player.knockbackMagnitude;
        player.SetVelocity(knockbackVel);
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("exit");

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
