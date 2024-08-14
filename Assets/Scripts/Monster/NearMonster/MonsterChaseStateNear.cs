using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterChaseStateNear : MonsterState
{
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
        monster.SpeedToZero();
    }

    public override void Update()
    {
        if (monster.isDead) Exit();
        else if (!monster.isSpawned) return;

        base.Update();

        if (monster.isChase)
        {   //navigate
            monster.agent.SetDestination(player.transform.position);
            Direction newDir = monster.CheckDir();
            if (monster.isFirst)
            {
                monster.isFirst = false;
                monster.monsterAnimController.SetAnim(MonsterAnimState.Run, newDir);
            }
            else if (curDir != newDir)
            {   // 플레이어를 쫓아가는 방향이 달라지면 새로운 애니메이션 호출
                curDir = newDir;
                monster.monsterAnimController.SetAnim(MonsterAnimState.Run, curDir);
            }
        }

        if (monster.distanceToPlayer < monster.attackRange && !monster.inAttack && monster.tempcool<=0.0)
            stateMachine.ChangeState(monster.attackState);
    }
}
