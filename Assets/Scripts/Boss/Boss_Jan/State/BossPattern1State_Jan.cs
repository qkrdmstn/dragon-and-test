using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public enum PathType
{
    straight,
    VLine
};

public class BossPattern1State_Jan : BossState_Jan
{
    public BossPattern1State_Jan(Boss_Jan _boss, BossStateMachine _stateMachine, Player _player) : base(_boss, _stateMachine, _player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        boss.SetVelocity(Vector2.zero);
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

            Vector2 shootDir = Quaternion.AngleAxis((360 / boss.sphereShootNum) * j /*+ (180 / boss.sphereShootNum)*/, Vector3.forward) * Vector3.right;
            shootDir.Normalize();

            bullet.BulletInitialize(shootDir, boss.pattern1BulletSpeed);
        }
        yield return new WaitForSeconds(boss.spherePathInterval);

        //4등분 Path Shoot 
        boss.StartCoroutine(Pattern1PathShoot(Vector3.right, PathType.VLine));
        boss.StartCoroutine(Pattern1PathShoot(Vector3.up, PathType.straight));
        boss.StartCoroutine(Pattern1PathShoot(Vector3.left, PathType.VLine));
        boss.StartCoroutine(Pattern1PathShoot(Vector3.down, PathType.straight));
    }

    IEnumerator Pattern1PathShoot(Vector3 pivotDir, PathType pathType)
    {
        int[,] bullets = GetPathTypeMat(pathType);

        int bulletRowNum = bullets.GetLength(0);
        int bulletColNum = bullets.GetLength(1);
        for (int i = bulletRowNum - 1; i >= 0; i--)
        {
            for (int j = 0; j < bulletColNum; j++)
            {
                if (bullets[i, j] == 0)
                    continue;
                //Shoot
                GameObject bulletObject = GameObject.Instantiate(boss.bulletPrefab, boss.transform.position, Quaternion.identity);
                BossBullet_Jan bullet = bulletObject.GetComponent<BossBullet_Jan>();

                Vector2 shootDir = Quaternion.AngleAxis((90 / bulletColNum) * j, Vector3.forward) * pivotDir;
                shootDir.Normalize();

                bullet.BulletInitialize(shootDir, boss.pattern1BulletSpeed, boss.pathBulletLifeTime);
            }
            yield return new WaitForSeconds(boss.pathInterval);
        }
    }

    private int[,] GetPathTypeMat(PathType pathType)
    {
        int[,] bullets = new int[15, 30];
        if (pathType == PathType.straight)
        {
            bullets = new int[15, 30]
            {
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1}
            };
        }
        else if (pathType == PathType.VLine)
        {
            bullets = new int[15, 30]
            {
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1},
                {1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1},
                {1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1}
            };
        }

        return bullets;
    }
}
