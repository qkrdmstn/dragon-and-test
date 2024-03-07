using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackableState : PlayerState
{
    public PlayerAttackableState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        if (true) // To do. 전투 가능한 곳 조건 추가
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
                player.gun.Shoot();
            
        }
    }
}
