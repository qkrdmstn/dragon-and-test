using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    private Player player;
    public PlayerState currentState { get; private set; }

    public PlayerStateMachine(Player _player)
    {
        player = _player;
    }

    public void Initialize(PlayerState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    public void ChangeState(PlayerState _newState)
    {
        if (player.isStateChangeable)
        {
            currentState.Exit();
            currentState = _newState;
            currentState.Enter();
        }
    }
}
