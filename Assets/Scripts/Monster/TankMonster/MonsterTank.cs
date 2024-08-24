using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterTank : MonsterNear
{
    public float fireInterval = 0.2f;
    public int bulletCount = 16;
    private Vector3 dir;
    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    protected override IEnumerator AnimSpawn()
    {
        isSpawned = true;
        SpeedReturn();
        yield return null;
    }

    protected override IEnumerator AnimDead()
    {
        yield return null;
    }

    public override void Dead()
    {
        if (!isDead)
        {
            isChase = false;
            isDead = true;
            if (ScenesManager.instance.GetSceneEnum() != SceneInfo.Boss_1 && SceneManager.GetActiveScene().name != "BossTest")
            {
                spawn.DeathCount();
                ItemDrop();
            }
            Destroy(gameObject);
        }
    }

    public override void Attack()
    {
        inAttack = true;
        AttackPoint();
    }

    public override void AttackPoint()
    {
        dir = player.transform.position - transform.position;

        StartCoroutine(SpawnBullets());
    }

    IEnumerator SpawnBullets()
    {
        for (int i=0; i < 3; i++)
        {
            for (int j = 0; j < bulletCount; j++)
            {
                var bulletGo = MonsterPool.instance.pool.Get();
                var bulletComponent = bulletGo.GetComponent<MonsterBullet>();
                bulletGo.transform.position = transform.position - new Vector3(0, 0.6f, 0);

                bulletComponent.BulletInitialize(Quaternion.AngleAxis((360 / bulletCount) * j + (180 / bulletCount) * (i % 2), Vector3.forward) * dir);
            }
            yield return new WaitForSeconds(fireInterval);
        }

        OutAttack();
    }
}