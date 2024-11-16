using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateMachine
{
    private MonsterBase monster;
    public MonsterState currentState { get; private set; }

    public MonsterStateMachine(MonsterBase _monster)
    {
        monster = _monster;
    }

    public void Initialize(MonsterState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    public void ChangeState(MonsterState _newState)
    {
        if(monster.isStateChangeable)
        {
            currentState.Exit();
            currentState = _newState;
            currentState.Enter();
        }
    }
}