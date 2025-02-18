using System.Collections;
using System.Collections.Generic;
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
            player.OnDamamged(1);

        //Physics2D.
        pattern3Object.ObjectSetActive(false);
        stateMachine.ChangeState(monster.idleState);
    }
}
