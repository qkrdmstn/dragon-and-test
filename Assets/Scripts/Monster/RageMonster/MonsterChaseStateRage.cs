using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseStateRage : MonsterState2
{
    private Vector2 direction;
    private MonsterRage monster;
    public MonsterChaseStateRage(MonsterStateMachine2 _stateMachine, GameObject _player, MonsterRage _monster) : base(_stateMachine, _player)
    {
        monster = _monster;
    }

    public override void Enter()
    {
        base.Enter();
        monster.SpeedReturn();
    }

    public override void Exit()
    {
        base.Exit();
        monster.SpeedToZero();
    }

    public override void Update()
    {
        base.Update();

        //navigate
        //monster.agent.SetDestination(player.transform.position);
        
        if (monster.distanceToPlayer < monster.rageRange && monster.rageAble)
            stateMachine.ChangeState(monster.rageState);
    }
}
