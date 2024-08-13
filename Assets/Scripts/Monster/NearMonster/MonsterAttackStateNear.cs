using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackStateNear : MonsterState
{
    private MonsterNear monster;
    public MonsterAttackStateNear(MonsterStateMachine _stateMachine, GameObject _player, MonsterNear _monster) : base(_stateMachine, _player)
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
        if (!monster.isSpawned) return;
        monster.SpeedReturn();
    }

    public override void Update()
    {
        if(monster.isDead) return;
        else if (!monster.isSpawned) return;

        base.Update();

        //Attack
        monster.tempcool -= Time.deltaTime;
        if (monster.tempcool <= 0.00 && !monster.inAttack) 
        {
            if (monster.distanceToPlayer > monster.attackRange)
                stateMachine.ChangeState(monster.chaseState);

            else if(!monster.isKnockedBack)
            {
                monster.tempcool = monster.cooldown;
                monster.monsterAnimController.SetAnim(MonsterAnimState.Attack, monster.CheckDir());
                monster.Attack();
            }
        }
    }
}
