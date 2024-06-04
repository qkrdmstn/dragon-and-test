using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTank : MonsterNear
{
    public float fireInterval = 0.2f;
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
    
    public override void Attack()
    {
        base.Attack();
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
                for (int j=0; j < 12; j++)
                {
                    var bulletGo = MonsterPool.instance.pool.Get();
                    var bulletComponent = bulletGo.GetComponent<MonsterBullet>();
                    bulletGo.transform.position = transform.position - new Vector3(0, 0.6f, 0);

                    bulletComponent.BulletInitialize(Quaternion.AngleAxis(30*(j-5.5f), Vector3.forward) * dir);
                }
                yield return new WaitForSeconds(fireInterval);
            }
    }


    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            OnDamaged(1);
        }
    }

}