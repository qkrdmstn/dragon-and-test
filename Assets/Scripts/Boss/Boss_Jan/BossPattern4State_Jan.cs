using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossPattern4State_Jan : BossState_Jan
{
    public BossPattern4State_Jan(Boss_Jan _boss, BossStateMachine _stateMachine, Player _player) : base(_boss, _stateMachine, _player)
    {

    }

    public override void Enter()
    {
        base.Enter();

        boss.StartCoroutine(Pattern4PathShoot());

    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();
    }

    IEnumerator Pattern4PathShoot()
    {
        List<GameObject> bulletObjects = new List<GameObject>();

        int[,] bulletsMat = GetPathTypeMat();

        int bulletRowNum = bulletsMat.GetLength(0);
        int bulletColNum = bulletsMat.GetLength(1);
        for (int i = 0; i < bulletRowNum; i++)
        {
            for (int j = 0; j < bulletColNum; j++)
            {
                if (bulletsMat[i, j] == 0)
                    continue;
                //Shoot
                float bulletXPos = boss.transform.position.x + (j - (bulletColNum / 2)) * boss.pattern4StartInterval;
                float bulletYPos = boss.transform.position.y + ((bulletRowNum / 2) - i) * boss.pattern4StartInterval;
                Vector3 bulletPos = new Vector3(bulletXPos, bulletYPos, boss.transform.position.z);
                GameObject bulletObject = GameObject.Instantiate(boss.bulletPrefab, bulletPos, Quaternion.identity);
                bulletObjects.Add(bulletObject);
                BossBullet_Jan bullet = bulletObject.GetComponent<BossBullet_Jan>();

                bullet.BulletInitialize(Vector2.down, 0.0f, 100.0f);
            }
        }
        yield return new WaitForSeconds(2.0f);

        int bulletIndex = 0;
        for (int i = 0; i < bulletRowNum; i++)
        {
            for (int j = 0; j < bulletColNum; j++)
            {
                if (bulletsMat[i, j] == 0)
                    continue;
                //Shoot
                float bulletXPos = boss.transform.position.x + (j - (bulletColNum / 2)) * boss.pattern4EndInterval;
                float bulletYPos = boss.transform.position.y + ((bulletRowNum / 2) - i) * boss.pattern4EndInterval;
                Vector3 bulletPos = new Vector3(bulletXPos, bulletYPos, boss.transform.position.z);
                bulletObjects[bulletIndex].transform.DOMove(bulletPos, 0.5f);
                bulletIndex++;
            }
        }
        yield return new WaitForSeconds(2.0f);

        bulletIndex = 0;
        for (int i = 0; i < bulletRowNum; i++)
        {
            for (int j = 0; j < bulletColNum; j++)
            {
                if (bulletsMat[i, j] == 0)
                    continue;
                Vector2 shootDir = Vector2.down;
                shootDir.Normalize();

                BossBullet_Jan bullet = bulletObjects[bulletIndex].GetComponent<BossBullet_Jan>();
                bullet.BulletInitialize(shootDir, boss.pattern4BulletSpeed, 100.0f);
                bulletIndex++;
            }
        }
        yield return new WaitForSeconds(2.0f);

        stateMachine.ChangeState(boss.bossIdleState);
    }

    private int[,] GetPathTypeMat()
    {

        int[,] bullets = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,1,1,0,0,1,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,1,0,0,1,1,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,1,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,1,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,1,1,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0},
                {0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0},
                {0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0},
                {0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0},
                {0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0},
                {0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,0,1,1,0,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0,1,0,0,0,0,1,1,0,0,0,0,1,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,0,1,1,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,1,0,0,0,1,0,0,0,1,0,1,1,1,1,0,0,0,0,1,1,0,0,0,0,1,1,1,1,0,1,0,0,0,1,0,0,0,1,0,0,0,0,0,1},
                {1,0,0,0,0,1,0,0,0,1,0,1,0,1,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,1,0,1,0,1,0,0,0,1,0,0,0,0,1},
                {1,1,1,1,1,0,1,1,1,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,1,1,1,0,1,1,1,1,1},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}

            };


        return bullets;
    }
}
