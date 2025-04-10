using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterPattern4State_Jan : MonsterState
{
    private new Boss_Jan monster;
    public MonsterPattern4State_Jan(MonsterStateMachine _stateMachine, Player _player, Boss_Jan _boss) : base(_stateMachine, _player, _boss)
    {
        monster = _boss;
    }

    public override void Enter()
    {
        base.Enter();

        monster.StartCoroutine(Pattern4PathShoot());

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
        Vector2Int blockMaxGrid = monster.bossField.GetMaxGridPos();
        monster.transform.position = monster.bossField.GridToWorldPosition(new Vector2(blockMaxGrid.x / 2, blockMaxGrid.y - 4));
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
                float bulletXPos = monster.transform.position.x + (j - (bulletColNum / 2)) * monster.pattern4StartInterval;
                float bulletYPos = monster.transform.position.y + ((bulletRowNum / 2) - i) * monster.pattern4StartInterval;
                Vector3 bulletPos = new Vector3(bulletXPos, bulletYPos, monster.transform.position.z);
                GameObject bulletObject = GameObject.Instantiate(monster.bulletPrefab, bulletPos, Quaternion.identity);
                bulletObjects.Add(bulletObject);

                if(bulletObject != null)
                {
                    BossBullet_Jan bullet = bulletObject.GetComponent<BossBullet_Jan>();
                    bullet.BulletInitialize(Vector2.down, 0.0f, 100.0f);
                }
            }
        }
        SoundManager.instance.SetEffectSound(SoundType.Boss, BossSfx.JanWingsCharge);

        yield return new WaitForSeconds(monster.pattern4BulletSpreadDelay);

        SoundManager.instance.SetEffectSound(SoundType.Boss, BossSfx.JanWingsAttack);
        int bulletIndex = 0;
        for (int i = 0; i < bulletRowNum; i++)
        {
            for (int j = 0; j < bulletColNum; j++)
            {
                if (bulletsMat[i, j] == 0)
                    continue;
                //Shoot
                float bulletXPos = monster.transform.position.x + (j - (bulletColNum / 2)) * monster.pattern4EndInterval;
                float bulletYPos = monster.transform.position.y + ((bulletRowNum / 2) - i) * monster.pattern4EndInterval;
                Vector3 bulletPos = new Vector3(bulletXPos, bulletYPos, monster.transform.position.z);
                if (bulletObjects[bulletIndex] != null)
                    bulletObjects[bulletIndex].transform.DOMove(bulletPos, 0.5f);
                bulletIndex++;
            }
        }
        yield return new WaitForSeconds(monster.pattern4BulletMoveDelay);

        bulletIndex = 0;
        for (int i = 0; i < bulletRowNum; i++)
        {
            for (int j = 0; j < bulletColNum; j++)
            {
                if (bulletsMat[i, j] == 0)
                    continue;
                Vector2 shootDir = Vector2.down;
                shootDir.Normalize();

                if(bulletObjects[bulletIndex] != null)
                {
                    BossBullet_Jan bullet = bulletObjects[bulletIndex].GetComponent<BossBullet_Jan>();
                    if (bullet != null)
                        bullet.BulletInitialize(shootDir, monster.pattern4BulletSpeed, 100.0f);
                }
                bulletIndex++;
            }
        }
        yield return new WaitForSeconds(2.0f);

        stateMachine.ChangeState(monster.idleState);
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
