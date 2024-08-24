using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossPattern2State_Jan : BossState_Jan
{
    private BlockInfo bossField;
    private Vector2Int maxGridPos;
    private int impactLayerMask = LayerMask.GetMask("Player");

    public BossPattern2State_Jan(Boss_Jan _boss, BossStateMachine _stateMachine, Player _player) : base(_boss, _stateMachine, _player)
    {
        bossField = boss.bossField;
        maxGridPos = bossField.GetMaxGridPos();
    }

    public override void Enter()
    {
        base.Enter();

        boss.isPattern2 = true;
        boss.SetVelocity(Vector2.zero);
        boss.StartCoroutine(Pattern2Reload());
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();
    }

    IEnumerator Pattern2Reload()
    {
        yield return new WaitForSeconds(boss.pattern2Delay);
        //Todo. isPattern2 == true 일 경우, idle, chase, basic attack state 상태만 가능하도록 막기
        stateMachine.ChangeState(boss.bossIdleState);
        
        int bulletNum = boss.pattern2ShootNum;
        float shootTimer = 0.0f;
        while (bulletNum > 0)
        {
            if (shootTimer <= 0.0f)
            {
                shootTimer = boss.pattern2ShootDelay;
                bulletNum--;
                //Shoot

                boss.StartCoroutine(Pattern2Shoot());
            }
            shootTimer -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Pattern2Shoot()
    {
        Vector2Int shootGridPos = GetShootGridPos();
        Vector3 shootPos = bossField.GridToWorldPosition(shootGridPos);
        GameObject displayObj = GameObject.Instantiate(boss.displayPrefab, shootPos, Quaternion.identity);

        //Display Delay
        float displayTimer = boss.pattern2Delay;
        while (displayTimer >= 0.0)
        {
            displayTimer -= Time.deltaTime;
            yield return null;
        }
        GameObject.Destroy(displayObj);


        Collider2D[] inRangeTarget = Physics2D.OverlapCircleAll(shootPos, boss.pattern2AttackRange, impactLayerMask);
        for (int i = 0; i < inRangeTarget.Length; i++)
        {
            GameObject target = inRangeTarget[i].gameObject;
            if (target.CompareTag("Player"))
            {
                Player.instance.OnDamamged(1);
            }
        }
    }

    private Vector2Int GetShootGridPos()
    {
        Vector2Int shootGridPos;

        Vector2Int playerGridPos = bossField.WorldToGridPosition(player.transform.position);
        int gridPosX = Random.Range(Mathf.Max(0, playerGridPos.x - boss.pattern2ShootOffset), Mathf.Min(playerGridPos.x + boss.pattern2ShootOffset, maxGridPos.x));
        int gridPosY = Random.Range(Mathf.Max(0, playerGridPos.y - boss.pattern2ShootOffset), Mathf.Min(playerGridPos.y + boss.pattern2ShootOffset, maxGridPos.y));
        shootGridPos = new Vector2Int(gridPosX, gridPosY);

        return shootGridPos;
    }
}
