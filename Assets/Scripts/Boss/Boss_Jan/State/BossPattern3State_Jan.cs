using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern3State_Jan : BossState_Jan
{
    private Pattern3Object pattern3Object;

    public BossPattern3State_Jan(Boss_Jan _boss, BossStateMachine _stateMachine, Player _player) : base(_boss, _stateMachine, _player)
    {
        pattern3Object = _boss.pattern3Object;
    }

    public override void Enter()
    {
        base.Enter();
        boss.SetVelocity(Vector2.zero);
        boss.StartCoroutine(Pattern3());
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
        boss.transform.position = Vector3.zero;
        yield return new WaitForSeconds(boss.pattern3Delay);

        //회전
        yield return new WaitForSeconds(boss.pattern3RotationTime);

        //공격 범위 & 생존 범위 표시
        float degree = Random.Range(0.0f, 360.0f);
        pattern3Object.SetSafeZone(degree);
        pattern3Object.ObjectSetActive(true);
        yield return new WaitForSeconds(boss.pattern3DisplayTime);

        //플레이어 데미지
        if (pattern3Object.IsInSafeZone())
            Debug.Log("is Safe");
        else
            player.OnDamamged(1);

        //Physics2D.
        pattern3Object.ObjectSetActive(false);
        stateMachine.ChangeState(boss.bossIdleState);
    }
}
