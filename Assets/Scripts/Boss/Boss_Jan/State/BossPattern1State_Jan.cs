using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BossPattern1State_Jan : BossState_Jan
{
    public BossPattern1State_Jan(Boss_Jan _boss, BossStateMachine _stateMachine, Player _player) : base(_boss, _stateMachine, _player)
    {
    }

    public override void Enter()
    {
        base.Enter();

        boss.StartCoroutine(Pattern1Shoot());
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        boss.SetVelocity(Vector2.zero);
    }

    IEnumerator Pattern1Shoot()
    {
        for (int j = 0; j < boss.sphereShootNum; j++)
        {
            //Shoot
            GameObject bulletObject = GameObject.Instantiate(boss.bulletPrefab, boss.transform.position, Quaternion.identity);
            BossBullet_Jan bullet = bulletObject.GetComponent<BossBullet_Jan>();

            Vector2 shootDir = Quaternion.AngleAxis((360 / boss.sphereShootNum) * j + (180 / boss.sphereShootNum), Vector3.forward) * Vector3.right;
            shootDir.Normalize();

            bullet.BulletInitialize(shootDir, boss.pattern1BulletSpeed);
        }
        yield return new WaitForSeconds(boss.sphereRandomInterval);

        for(int i=0; i<boss.waveNum; i++)
        {
            for (int j = 0; j < boss.randomShootNum; j++)
            {
                //Shoot
                GameObject bulletObject = GameObject.Instantiate(boss.bulletPrefab, boss.transform.position, Quaternion.identity);
                BossBullet_Jan bullet = bulletObject.GetComponent<BossBullet_Jan>();

                Vector2 shootDir = Quaternion.AngleAxis((360 / boss.randomShootNum) * j + (180 / boss.randomShootNum), Vector3.forward) * Vector3.right;
                shootDir.Normalize();

                bullet.BulletInitialize(shootDir, boss.pattern1BulletSpeed);
            }
            yield return new WaitForSeconds(boss.randomInterval);
        }


    }
}
