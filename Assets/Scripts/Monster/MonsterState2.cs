using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterState2
{
    #region Components
    protected MonsterStateMachine2 stateMachine;
    protected GameObject player;
    protected Direction curDir;
    #endregion

    public MonsterState2(MonsterStateMachine2 _stateMachine, GameObject _player)
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
