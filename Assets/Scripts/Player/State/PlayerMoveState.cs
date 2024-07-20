using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, AnimState _animStateName) : base(_player, _stateMachine, _animStateName)
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
        //Change State
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            stateMachine.ChangeState(player.dashState);
            return;
        }

        if (xInput == 0 && yInput == 0)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        base.Update();

        //Move
        Vector2 dir = new Vector2(xInput, yInput);
        dir.Normalize();

        Vector2 move = dir * player.moveSpeed;
        player.SetVelocity(move.x, move.y);
        player.animController.SetAnim(AnimState.Run, xInput, yInput);

        SoundManager.instance.PlayWalkEffect();
    }
}
