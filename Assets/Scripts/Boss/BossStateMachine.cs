using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine : MonoBehaviour
{
    public BossState currentState { get; private set; }

    public BossStateMachine()
    {
    }

    public void Initialize(BossState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    public void ChangeState(BossState _newState)
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}
