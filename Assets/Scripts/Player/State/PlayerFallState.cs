using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerState
{
    public PlayerFallState(Player _player, PlayerStateMachine _stateMachine, PlayerAnimState _animStateName) : base(_player, _stateMachine, _animStateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //낙하 상태 duration
        stateTimer = 1.0f;

        //속도 0 설정
        player.SetVelocity(0.0f, 0.0f);

        //낙하 데미지
        player.OnDamaged(1);
    }

    public override void Exit()
    {
        base.Exit();

        player.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        LocationRecovery();
        player.isFall = false;

        if (player.isCombatZone)
            player.isAttackable = true;
    }

    public override void Update()
    {
        base.Update();

        //scale 조정을 통한 낙하 연출 (애니메이션으로 대체)
        player.transform.localScale -= new Vector3(2.0f, 2.0f, 0.0f) * Time.deltaTime;
        if (player.transform.localScale.x < 0.0f)
            player.transform.localScale = new Vector3(0.0f, 0.0f, 1.0f);

        if (stateTimer < 0.0f)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    private void LocationRecovery()
    {
        Stack<Vector2> positionStack = new Stack<Vector2>(player.positionHistoryQueue);
        int count = positionStack.Count;
        for (int i = 0; i < count; i++)
        {
            //recoveryPosition 낭떠러지 check
            bool isCliff = false;
            Vector2 recoveryPosition = positionStack.Pop();
            Collider2D[] inRangeTarget = Physics2D.OverlapBoxAll(recoveryPosition, new Vector2(1.0f, 1.0f), 0.0f);
            for (int j = 0; j < inRangeTarget.Length; j++)
            {
                GameObject target = inRangeTarget[j].gameObject;
                if (target.CompareTag("Cliff"))
                {
                    isCliff = true;
                    break;
                }
            }
            if (!isCliff)
            {
                player.transform.position = recoveryPosition;
                player.positionSaveTimer = player.positionSaveInterval * 2;
                break;
            }
        }
    }
}
