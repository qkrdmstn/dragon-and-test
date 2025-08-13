using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterRageState_BirdRage : MonsterState
{
    protected new BirdRage monster;
    protected Direction curDir;
    private MonsterAnimController monsterAnimController;
    private GameObject attackRangeObj;
    private Collider2D attackRangeCol;
    private float attackCoolTimer;

    ContactFilter2D filter;
    public MonsterRageState_BirdRage(MonsterStateMachine _stateMachine, Player _player, BirdRage _monster) : base(_stateMachine, _player, _monster)
    {
        this.monster = _monster;

        if(monster.haveAnim)
            monsterAnimController = monster.monsterAnimController;
        attackRangeObj = monster.attackRangeObject;
        attackRangeCol = attackRangeObj.GetComponent<Collider2D>();

        filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Player"));

        curDir = Direction.FRONT;
    }

    public override void Enter()
    {
        base.Enter();
        monster.SetSpeed(monster.rageSpeed);
        attackRangeObj.SetActive(true);
    }

    public override void Update()
    {
        base.Update();
        monster.SetDestination(player.transform.position);

        //애니메이션 설정
        Direction newDir = monster.CheckDir();
        if (curDir != newDir)
        {   // 플레이어를 쫓아가는 방향이 달라지면 새로운 애니메이션 호출
            curDir = newDir;
            monster.monsterAnimController.SetAnim(MonsterAnimState.Attack, curDir, true);
        }

        //Attack Range Object 방향 설정
        Vector3 dir = player.transform.position - monster.transform.position;
        dir.Normalize();
        float theta = Vector2.Angle(Vector2.right, dir);
        if (dir.y < 0)
            theta *= -1;
        attackRangeObj.transform.rotation = Quaternion.Euler(0, 0, theta);
        attackRangeObj.transform.position = monster.transform.position + dir * 0.75f;

        //공격
        List<Collider2D> inRangeTarget = new List<Collider2D>();
        if (attackCoolTimer < 0.0f && Physics2D.OverlapCollider(attackRangeCol, filter.NoFilter(), inRangeTarget) != 0)
        {
            for (int i = 0; i < inRangeTarget.Count; i++)
            {
                if (!inRangeTarget[i].CompareTag("Player"))
                    continue;
                attackCoolTimer = monster.attackCoolTime;
                player.OnDamamged(1);
            }
        }

        attackCoolTimer -= Time.deltaTime;
    }

    public override void Exit()
    {
        base.Exit();
        monster.SetSpeed(0.0f);
        attackRangeObj.SetActive(false);
    }

}
