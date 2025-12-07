using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterEscapeState_BirdCrossbowman : MonsterState
{
    protected new BirdCrossbowman monster;
    protected Direction curDir;
    private Vector3 dest;
    private float timer;

    public MonsterEscapeState_BirdCrossbowman(MonsterStateMachine _stateMachine, Player _player, BirdCrossbowman _monster) : base(_stateMachine, _player, _monster)
    {
        monster = _monster;
        curDir = Direction.FRONT;
    }

    public override void Enter()
    {
        base.Enter();
        if (!monster.statusEffectsFlag.rooted) //속박 시 속도 변화 x
            monster.SetSpeed(monster.escapeSpeed);
        else
            monster.SetSpeed(0);

        timer = 0.0f;
    }

    public override void Update()
    {
        base.Update();

        //도망 방향 설정
        timer -= Time.deltaTime;
        if(timer < 0.0f)
        {
            timer = Random.Range(monster.escapeDirChangePeriod.x, monster.escapeDirChangePeriod.y);
            Vector3 dir = (monster.transform.position - player.transform.position).normalized;
            dir = Quaternion.AngleAxis(Random.Range(-monster.escapeAngleOffset, monster.escapeAngleOffset), Vector3.forward) * dir;
            dest = dir * monster.escapeDist;
        }
        monster.SetDestination(dest);

        Direction newDir = monster.CheckDirReverse();
        if (curDir != newDir)
        {   // 플레이어를 쫓아가는 방향이 달라지면 새로운 애니메이션 호출
            curDir = newDir;
            monster.monsterAnimController.SetAnim(TankerAnimState.Run, curDir);
        }

        //다음 상태 변경 확인
        float dist = Vector3.Distance(monster.transform.position, player.transform.position);
        if(dist > monster.escapeDist)
        {
            if (dist > monster.attackDist)
                stateMachine.ChangeState(monster.chaseState);
            else
                stateMachine.ChangeState(monster.attackState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        monster.SetSpeed(0.0f);
    }
}
