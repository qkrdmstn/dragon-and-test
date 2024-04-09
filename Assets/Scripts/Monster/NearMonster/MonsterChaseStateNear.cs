using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseStateNear : MonsterState
{
    private Vector2 direction;
    private MonsterNear monster;
    public MonsterChaseStateNear(MonsterStateMachine _stateMachine, GameObject _player, MonsterNear _monster) : base(_stateMachine, _player)
    {
        monster = _monster;
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
        if(monster.distanceToPlayer>1f) monster.transform.Translate(direction * monster.tempSpeed * Time.deltaTime);
        
        if (monster.distanceToPlayer < monster.attackRange && monster.tempcool<=0.0)
            stateMachine.ChangeState(monster.attackState);
    }
}
