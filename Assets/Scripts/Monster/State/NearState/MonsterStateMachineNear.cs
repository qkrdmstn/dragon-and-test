using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateMachineNear
{
    public MonsterStateNear currentState { get; private set; }

    public void Initialize(MonsterStateNear _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    public void ChangeState(MonsterStateNear _newState)
    {
        if (currentState == _newState) return;
        if (currentState != null) currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}
