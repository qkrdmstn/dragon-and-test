using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterBasicAttackState_Jan : MonsterState
{
    private new Boss_Jan monster;
    public MonsterBasicAttackState_Jan(MonsterStateMachine _stateMachine, Player _player, Boss_Jan _boss) : base(_stateMachine, _player, _boss)
    {
        monster = _boss;
    }

    public override void Enter()
    {
        base.Enter();

        monster.reloadCnt++;
        monster.StartCoroutine(AttackCoroutine());
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }

    IEnumerator AttackCoroutine()
    {
        float shootTimer = 0.0f;
        while(monster.loadedBullet > 0)
        {
            if(shootTimer <= 0.0f)
            {
                shootTimer = monster.shootDelay;
                monster.loadedBullet--;

                Shoot();
            }
            shootTimer -= Time.deltaTime;
            yield return null;
        }

        monster.StartCoroutine(ReloadCoroutine());
    }

    private void Shoot()
    {
        //Shoot
        GameObject bulletObject = GameObject.Instantiate(monster.bulletPrefab, monster.transform.position, Quaternion.identity);
        BossBullet_Jan bullet = bulletObject.GetComponent<BossBullet_Jan>();

        Vector2 shootDir = player.transform.position - monster.transform.position;
        shootDir.Normalize();

        //반동 설정
        float degree = Random.Range(-monster.maxRecoilDegree, monster.maxRecoilDegree);
        shootDir = Quaternion.AngleAxis(degree, Vector3.forward) * shootDir;

        bullet.BulletInitialize(shootDir, monster.bulletSpeed, monster.bulletRange);
    }

    IEnumerator ReloadCoroutine()
    {
        float reloadTimer = monster.reloadTime;

        while (true)
        {
            reloadTimer -= Time.deltaTime;
            yield return null;

            if (reloadTimer <= 0.0f)
            {
                monster.loadedBullet = monster.magazineSize;
                break;
            }
        }

        if(monster.reloadCnt % 2 == 0)
        {
            float randomVal = Random.Range(0.0f, 1.0f);
            if (randomVal <= monster.pattern1Prob)
                stateMachine.ChangeState(monster.pattern1State);
            else if (randomVal <= monster.pattern1Prob + monster.pattern2Prob)
                stateMachine.ChangeState(monster.pattern2State);            
            else if (randomVal <= monster.pattern1Prob + monster.pattern2Prob + monster.pattern3Prob)
                stateMachine.ChangeState(monster.pattern3State);            
            else if (randomVal <= monster.pattern1Prob + monster.pattern2Prob + monster.pattern3Prob + monster.pattern4Prob)
                stateMachine.ChangeState(monster.pattern4State);
        }
        else
            stateMachine.ChangeState(monster.idleState);
    }
}
