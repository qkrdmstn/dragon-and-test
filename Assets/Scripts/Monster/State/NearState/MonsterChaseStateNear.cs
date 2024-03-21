using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseStateNear : MonsterStateNear
{
    private Vector2 direction;
    public MonsterChaseStateNear(MonsterNear _monster, MonsterStateMachineNear _stateMachine, GameObject _player) : base(_monster, _stateMachine, _player)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        direction = monster.chase.tempDir;
        if(distanceToPlayer>1f) monster.transform.Translate(direction * monster.tempSpeed * Time.deltaTime);
        
        //Debug.Log(distanceToPlayer);
        if (distanceToPlayer < monster.attackRange && monster.tempcool<=0.0)
            stateMachine.ChangeState(monster.attackState);
    }
}
