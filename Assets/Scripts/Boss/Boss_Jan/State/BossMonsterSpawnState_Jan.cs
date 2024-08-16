using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSpawnState_Jan : BossState_Jan
{
    private BlockInfo bossField;
    private Vector2Int maxGridPos;

    public BossMonsterSpawnState_Jan(Boss_Jan _boss, BossStateMachine _stateMachine, Player _player, BlockInfo _bossField) : base(_boss, _stateMachine, _player)
    {
        bossField = _bossField;
        maxGridPos = bossField.GetMaxGridPos();
    }

    public override void Enter()
    {
        base.Enter();

        SpawnMonster();
        stateTimer = boss.spawnDelay;
        boss.spawnTimer = boss.spawnPeriod;
        boss.waveCnt++;

        boss.SetVelocity(Vector2.zero);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        boss.SetVelocity(Vector2.zero);
        if (stateTimer < 0.0f)
            stateMachine.ChangeState(boss.bossIdleState);
    }

    private void SpawnMonster()
    {
        int spawnNum = 7;
        if (boss.waveCnt == 0)
            spawnNum = 4;
        else if (boss.waveCnt == 1)
            spawnNum = 6;

        Vector2Int[] spawnGridPos = GetSpawnGridPos(spawnNum);

        //몬스터 스폰
        for (int i=0; i < spawnNum; i++)
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
        while(cnt < spawnNum)
        {
            int gridPosX = Random.Range(0, maxGridPos.x);
            int gridPosY = Random.Range(0, maxGridPos.y);

            for (i = 0; i < cnt; i++)
            {
                if (spawnGridPos[i].x == gridPosX && spawnGridPos[i].y == gridPosY)
                    break;
            }
            if(i == cnt)
            {
                spawnGridPos[cnt] = new Vector2Int(gridPosX, gridPosY);
                cnt++;
            }
        }

        return spawnGridPos;
    }
}
