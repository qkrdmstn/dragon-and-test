using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterPattern3State_Jan : MonsterState
{
    private new Boss_Jan monster;
    private Pattern3Object pattern3Object;

    public MonsterPattern3State_Jan(MonsterStateMachine _stateMachine, Player _player, Boss_Jan _boss) : base(_stateMachine, _player, _boss)
    {
        monster = _boss;
        pattern3Object = _boss.pattern3Object;
    }

    public override void Enter()
    {
        base.Enter();
        monster.SetSpeed(0.0f);
        monster.StartCoroutine(Pattern3());
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }

    IEnumerator Pattern3()
    {
        monster.transform.position = monster.bossField.GetBlockCenter();
        yield return new WaitForSeconds(monster.pattern3Delay);

        //회전
        yield return new WaitForSeconds(monster.pattern3RotationTime);

        //공격 범위 & 생존 범위 표시
        float degree = Random.Range(0.0f, 360.0f);
        pattern3Object.SetSafeZone(degree);
        pattern3Object.ObjectSetActive(true);
        yield return new WaitForSeconds(monster.pattern3DisplayTime);

        SoundManager.instance.SetEffectSound(SoundType.Boss, BossSfx.JanFootStep);
        //플레이어 데미지
        if (pattern3Object.IsInSafeZone())
            Debug.Log("is Safe");
        else
            player.OnDamaged(1);

        //카메라 쉐이킹 연출 넣기
        //엄폐물 낙하지점 표시
        Vector2Int[] spawnGridPos = monster.GetRandomGridPos(monster.pattern3ObstacleNum);
        GameObject[] displayObjects = new GameObject[monster.pattern3ObstacleNum];
        for (int i = 0; i < monster.pattern3ObstacleNum; i++)
            displayObjects[i] = GameObject.Instantiate(monster.pattern3ObstacleDisplayPrefab, monster.bossField.GridToWorldPosition(spawnGridPos[i]), Quaternion.identity);
        yield return new WaitForSeconds(monster.pattern3ObstacleDisplayTime);
        for (int i = 0; i < monster.pattern3ObstacleNum; i++)
            GameObject.Destroy(displayObjects[i]);

        //엄폐물 생성
        for (int i = 0; i < monster.pattern3ObstacleNum; i++)
            GameObject.Instantiate(monster.pattern3ObstaclePrefab, monster.bossField.GridToWorldPosition(spawnGridPos[i]), Quaternion.identity);

        //Physics2D.
        pattern3Object.ObjectSetActive(false);
        stateMachine.ChangeState(monster.idleState);
    }
}
