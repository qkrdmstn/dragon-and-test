using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossState_Jan : BossState
{
    protected float stateTimer;

    protected Boss_Jan boss;

    public BossState_Jan(Boss_Jan _boss, BossStateMachine _stateMachine, Player _player) : base(_stateMachine, _player)
    {
        boss = _boss;
    }

    public override void Enter()
    {
        base.Enter();
    }


    public override void Update()
    {
        base.Update();
        stateTimer -= Time.deltaTime;
        boss.spawnTimer -= Time.deltaTime;

        if(boss.isDead)
            stateMachine.ChangeState(boss.bossIdleState);

        if (boss.spawnTimer < 0.0f)
            stateMachine.ChangeState(boss.bossMonsterSpawnState);
    }

    public override void Exit()
    {
        base.Exit();
        //player.anim.SetBool(animBoolName, false);
    }
}
