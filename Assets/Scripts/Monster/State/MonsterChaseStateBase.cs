using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseStateBase : MonsterState
{
    protected Direction curDir;

    public MonsterChaseStateBase(MonsterStateMachine _stateMachine, Player _player, MonsterBase _monster) : base(_stateMachine, _player, _monster)
    {
        curDir = Direction.FRONT;
    }

    public override void Enter()
    {
        base.Enter();
        monster.SetSpeed(monster.moveSpeed);
    }

    public override void Update()
    {
        base.Update();
        monster.SetDestination(player.transform.position);

        Direction newDir = monster.CheckDir();
        if (curDir != newDir)
        {   // 플레이어를 쫓아가는 방향이 달라지면 새로운 애니메이션 호출
            curDir = newDir;
            monster.monsterAnimController.SetAnim(MonsterAnimState.Run, curDir);
        }
    }

    public override void Exit()
    {
        base.Exit();
        monster.SetSpeed(0.0f);
    }


}
