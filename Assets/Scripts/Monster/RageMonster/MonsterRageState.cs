using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRageState : MonsterState2
{
    private MonsterRage monster;
    public MonsterRageState(MonsterStateMachine2 _stateMachine, GameObject _player, MonsterRage _monster) : base(_stateMachine, _player)
    {
        monster = _monster;
    }

    public override void Enter()
    {
        base.Enter();
        monster.SpeedBoost();
    }

    public override void Exit()
    {
        base.Exit();
        monster.SpeedReturn();
    }

    public override void Update()
    {
        base.Update();
        
        //monster.agent.SetDestination(player.transform.position);
        monster.tempcool -= Time.deltaTime;
        if (monster.tempcool<=0.0) 
        {
            if(!monster.isKnockedBack)
            {
                monster.tempcool = monster.cooldown;
                monster.Attack();
            }
        }
        
    }
}
