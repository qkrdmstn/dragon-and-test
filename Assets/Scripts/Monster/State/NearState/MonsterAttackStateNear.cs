using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackStateNear : MonsterStateNear
{
    public MonsterAttackStateNear(MonsterNear _monster, MonsterStateMachineNear _stateMachine, GameObject _player) : base(_monster, _stateMachine, _player)
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

        //Attack
        monster.tempcool -= Time.deltaTime;
        if (monster.tempcool<=0.0) 
        {
            if (distanceToPlayer > monster.attackRange && (!monster.inAttack)) stateMachine.ChangeState(monster.chaseState);
            else
            {
                monster.tempcool = monster.cooldown;
                monster.Attack();
            }
        }
        
    }
}
