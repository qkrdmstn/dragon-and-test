using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackStateDash : MonsterState
{
    private MonsterDash monster;
    public MonsterAttackStateDash(MonsterStateMachine _stateMachine, GameObject _player, MonsterDash _monster) : base(_stateMachine, _player)
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

        //Attack
        if (monster.tempcool<=0.0) 
        {
            if (monster.distanceToPlayer > monster.attackRange && (!monster.inAttack)) stateMachine.ChangeState(monster.idleState);
            else if(!monster.isKnockedBack)
            {
                monster.tempcool = monster.cooldown;
                monster.Attack();
                stateMachine.ChangeState(monster.idleState);
            }
        }
        
    }
}
