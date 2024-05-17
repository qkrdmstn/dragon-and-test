using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateMachine
{
    public MonsterState currentState { get; private set; }

    public void Initialize(MonsterState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    public void ChangeState(MonsterState _newState)
    {
        //if (currentState == _newState) return;
        if (currentState != null) currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}