using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterShotGun : MonsterFar
{
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
    
    public override void Shoot()
    {
        if(loadedBullet > 0 && monsterShootTimer < 0.0)
        {
            monsterShootTimer = monsterShootDelay;
            loadedBullet--;
            Vector3 dir = player.transform.position-transform.position;

            //Create Bullet
            for (int i=0; i < 3; i++)
            {
                var bulletGo = MonsterPool.instance.pool.Get();
                var bulletComponent = bulletGo.GetComponent<MonsterBullet>();
                bulletGo.transform.position = transform.position;
                
                bulletComponent.BulletInitialize(Quaternion.AngleAxis(Random.Range(-15, 15), Vector3.forward) * dir, Random.Range(3f, 7f));
            }

        } 
        else if (loadedBullet <= 0)
        {
            Reload();
        }
    }

}