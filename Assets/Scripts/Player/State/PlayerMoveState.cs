using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        //Move
        Vector2 dir = new Vector2(xInput, yInput);
        dir.Normalize();

        Vector2 move = dir * player.moveSpeed;
        player.SetVelocity(move.x, move.y);
        SoundManager.instance.PlayWalkEffect();

        //Change State
        if (Input.GetKeyDown(KeyCode.Mouse1))
            stateMachine.ChangeState(player.dashState);

        if (xInput == 0 && yInput == 0)
            stateMachine.ChangeState(player.idleState);
    }


}
