using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRageState : MonsterState
{
    private MonsterRage monster;
    public MonsterRageState(MonsterStateMachine _stateMachine, GameObject _player, MonsterRage _monster) : base(_stateMachine, _player)
    {
        monster = _monster;
    }

    public override void Enter()
    {
        base.Enter();
        monster.SpeedToZero();
    }

    public override void Exit()
    {
        base.Exit();
        monster.SpeedReturn();
    }

    public override void Update()
    {
        base.Update();

        //Attack
        monster.tempcool -= Time.deltaTime;
        if (monster.tempcool<=0.0) 
        {
            if (monster.distanceToPlayer > monster.attackRange && (!monster.inAttack)) stateMachine.ChangeState(monster.chaseState);
            else if(!monster.isKnockedBack)
            {
                monster.tempcool = monster.cooldown;
                monster.Attack();
            }
        }
        
    }
}
