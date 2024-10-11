using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnStateBase : MonsterState
{
    public MonsterSpawnStateBase(MonsterStateMachine _stateMachine, Player _player, MonsterBase _monster) : base(_stateMachine, _player, _monster)
    {
    }

    public override void Enter()
    {
        base.Enter();

        monster.StartCoroutine(SpawnCoroutine());
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }

    IEnumerator SpawnCoroutine()
    {
        monster.SetSpeed(0.0f);
        if (monster.haveAnim)
            monster.monsterAnimController.SetAnim();

        yield return new WaitForSeconds(monster.spawnDuration);

        stateMachine.ChangeState(monster.idleState);
    }
}
