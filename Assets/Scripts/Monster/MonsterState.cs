using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterState
{
    #region Components
    protected MonsterStateMachine stateMachine;
    protected GameObject player;
    #endregion
    
    public MonsterState(MonsterStateMachine _stateMachine, GameObject _player)
    {
        this.stateMachine = _stateMachine;
        this.player = _player;
    }

    public virtual void Enter()
    {
        return;
    }

    public virtual void Update()
    {
        return;
    }

    public virtual void Exit()
    {
        return;
    }

    
}
