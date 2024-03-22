using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateNear
{
    #region Components
    protected MonsterStateMachineNear stateMachine;
    protected MonsterNear monster;
    protected GameObject player;
    #endregion

    protected float distanceToPlayer;
    
    public MonsterStateNear(MonsterNear _monster, MonsterStateMachineNear _stateMachine, GameObject _player)
    {
        this.monster = _monster;
        this.stateMachine = _stateMachine;
        this.player = _player;
    }

    public virtual void Enter()
    {
        return;
    }

    public virtual void Update()
    {
        distanceToPlayer = Vector3.Distance(monster.transform.position, player.transform.position);
        
    }

    public virtual void Exit()
    {
        return;
    }

    
}
