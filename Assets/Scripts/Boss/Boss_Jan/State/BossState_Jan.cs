using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossState_Jan : BossState
{
    protected Boss_Jan boss;
    private BlockInfo bossField;
    private Vector2Int maxGridPos;

    public BossState_Jan(Boss_Jan _boss, BossStateMachine _stateMachine, Player _player) : base(_stateMachine, _player)
    {
        boss = _boss;
        bossField = boss.bossField;
        maxGridPos = bossField.GetMaxGridPos();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        boss.spawnTimer -= Time.deltaTime;

        if(boss.isDead)
            stateMachine.ChangeState(boss.bossIdleState);

        if (!boss.isDead && boss.spawnTimer < 0.0f)
            SpawnMonster();
    }

    public override void Exit()
    {
        base.Exit();
        //player.anim.SetBool(animBoolName, false);
    }

    private void SpawnMonster()
    {
        boss.spawnTimer = boss.spawnPeriod;

        int spawnNum = 3;
        //if (boss.spawnWaveCnt == 0)
        //    spawnNum = 4;
        //else if (boss.spawnWaveCnt == 1)
        //    spawnNum = 5;

        Vector2Int[] spawnGridPos = GetSpawnGridPos(spawnNum);

        //몬스터 스폰
        for (int i = 0; i < spawnNum; i++)
        {
            int idx = Random.Range(0, boss.spawnMosnterPrefabs.Length);
            GameObject.Instantiate(boss.spawnMosnterPrefabs[idx], bossField.GridToWorldPosition(spawnGridPos[i]), Quaternion.identity);
        }
    }

    private Vector2Int[] GetSpawnGridPos(int spawnNum)
    {
        Vector2Int[] spawnGridPos = new Vector2Int[spawnNum];

        int i = 0;
        int cnt = 0;
        while (cnt < spawnNum)
        {
            int gridPosX = Random.Range(0, maxGridPos.x);
            int gridPosY = Random.Range(0, maxGridPos.y);

            for (i = 0; i < cnt; i++)
            {
                if (spawnGridPos[i].x == gridPosX && spawnGridPos[i].y == gridPosY)
                    break;
            }
            if (i == cnt)
            {
                spawnGridPos[cnt] = new Vector2Int(gridPosX, gridPosY);
                cnt++;
            }
        }

        return spawnGridPos;
    }
}
