using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterPattern2State_Jan : MonsterState
{
    private new Boss_Jan monster;
    private BlockInfo bossField;
    private Vector2Int maxGridPos;
    private int impactLayerMask = LayerMask.GetMask("Player");

    public MonsterPattern2State_Jan(MonsterStateMachine _stateMachine, Player _player, Boss_Jan _boss) : base(_stateMachine, _player, _boss)
    {
        monster = _boss;
        bossField = monster.bossField;
        maxGridPos = bossField.GetMaxGridPos();
    }

    public override void Enter()
    {
        base.Enter();

        monster.isPattern2 = true;
        monster.SetSpeed(0.0f);
        monster.StartCoroutine(Pattern2Reload());
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
        yield return new WaitForSeconds(monster.pattern2Delay);
        //Todo. isPattern2 == true 일 경우, idle, chase, basic attack state 상태만 가능하도록 막기
        stateMachine.ChangeState(monster.idleState);
        
        int bulletNum = monster.pattern2ShootNum;
        float shootTimer = 0.0f;
        while (bulletNum > 0)
        {
            if (shootTimer <= 0.0f)
            {
                shootTimer = monster.pattern2ShootDelay;
                bulletNum--;
                //Shoot

                monster.StartCoroutine(Pattern2Shoot());
            }
            shootTimer -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Pattern2Shoot()
    {
        Vector2Int shootGridPos = GetShootGridPos();
        Vector3 shootPos = bossField.GridToWorldPosition(shootGridPos);
        GameObject displayObj = GameObject.Instantiate(monster.displayPrefab, shootPos, Quaternion.identity);

        //Display Delay
        float displayTimer = monster.pattern2Delay;
        while (displayTimer >= 0.0)
        {
            displayTimer -= Time.deltaTime;
            yield return null;
        }
        GameObject.Destroy(displayObj);
        SoundManager.instance.SetEffectSound(SoundType.Boss, BossSfx.JanWingBomb);

        Collider2D[] inRangeTarget = Physics2D.OverlapCircleAll(shootPos, monster.pattern2AttackRange, impactLayerMask);
        for (int i = 0; i < inRangeTarget.Length; i++)
        {
            GameObject target = inRangeTarget[i].gameObject;
            if (target.CompareTag("Player"))
            {
                Player.instance.OnDamaged(1);
            }
        }
    }

    private Vector2Int GetShootGridPos()
    {
        Vector2Int shootGridPos;

        Vector2Int playerGridPos = bossField.WorldToGridPosition(player.transform.position);
        int gridPosX = Random.Range(Mathf.Max(0, playerGridPos.x - monster.pattern2ShootOffset), Mathf.Min(playerGridPos.x + monster.pattern2ShootOffset, maxGridPos.x));
        int gridPosY = Random.Range(Mathf.Max(0, playerGridPos.y - monster.pattern2ShootOffset), Mathf.Min(playerGridPos.y + monster.pattern2ShootOffset, maxGridPos.y));
        shootGridPos = new Vector2Int(gridPosX, gridPosY);

        return shootGridPos;
    }
}
