using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class BossState
{
    #region Components
    protected BossStateMachine stateMachine;
    #endregion

    protected Player player;
    public BossState( BossStateMachine _stateMachine, Player _player)
    {
        this.stateMachine = _stateMachine;
        this.player = _player;
    }

    public virtual void Enter()
    {
       
    }

    public virtual void Update()
    {
    }

    public virtual void Exit()
    {
        //player.anim.SetBool(animBoolName, false);
    }
}
