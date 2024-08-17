using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BossBasicAttackState_Jan : BossState_Jan
{
    public BossBasicAttackState_Jan(Boss_Jan _boss, BossStateMachine _stateMachine, Player _player) : base(_boss, _stateMachine, _player)
    {
    }

    public override void Enter()
    {
        base.Enter();

        boss.reloadCnt++;
        boss.StartCoroutine(AttackCoroutine());
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
        while(boss.loadedBullet > 0)
        {
            if(shootTimer <= 0.0f)
            {
                shootTimer = boss.shootDelay;
                boss.loadedBullet--;

                Shoot();
            }
            shootTimer -= Time.deltaTime;
            yield return null;
        }

        boss.StartCoroutine(ReloadCoroutine());
    }

    private void Shoot()
    {
        //Shoot
        GameObject bulletObject = GameObject.Instantiate(boss.bulletPrefab, boss.transform.position, Quaternion.identity);
        BossBullet_Jan bullet = bulletObject.GetComponent<BossBullet_Jan>();

        Vector2 shootDir = player.transform.position - boss.transform.position;
        shootDir.Normalize();

        //반동 설정
        float degree = Random.Range(-boss.maxRecoilDegree, boss.maxRecoilDegree);
        shootDir = Quaternion.AngleAxis(degree, Vector3.forward) * shootDir;

        bullet.BulletInitialize(shootDir, boss.bulletSpeed);
    }

    IEnumerator ReloadCoroutine()
    {
        float reloadTimer = boss.reloadTime;

        while (true)
        {
            reloadTimer -= Time.deltaTime;
            yield return null;

            if (reloadTimer <= 0.0f)
            {
                boss.loadedBullet = boss.magazineSize;
                break;
            }
        }

        if(boss.reloadCnt % 3 == 0)
        {
            float randomVal = Random.Range(0.0f, 1.0f);
            if (randomVal <= boss.pattern1Prob)
                stateMachine.ChangeState(boss.BossPattern1State);
            else if (randomVal <= boss.pattern1Prob + boss.pattern2Prob)
                stateMachine.ChangeState(boss.BossPattern2State);            
            else if (randomVal <= boss.pattern1Prob + boss.pattern2Prob + boss.pattern3Prob)
                stateMachine.ChangeState(boss.BossPattern3State);
        }
        stateMachine.ChangeState(boss.bossIdleState);
    }
}
