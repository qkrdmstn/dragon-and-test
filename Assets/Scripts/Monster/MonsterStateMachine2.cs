using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateMachine2
{
    public MonsterState2 currentState { get; private set; }

    public void Initialize(MonsterState2 _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    public void ChangeState(MonsterState2 _newState)
    {
        //if (currentState == _newState) return;
        if (currentState != null) currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}