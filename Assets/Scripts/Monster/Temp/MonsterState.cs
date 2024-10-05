using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterState
{
    #region Components
    protected MonsterStateMachine stateMachine;
    #endregion

    protected Player player;
    protected MonsterBase monster;
    public MonsterState(MonsterStateMachine _stateMachine, Player player, MonsterBase monster)
    {
        this.stateMachine = _stateMachine;
        this.player = player;
        this.monster = monster;
    }

    public virtual void Enter()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void Exit()
    {
    }
}
