using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterShotGun : MonsterFar
{
    private GameObject bulletGo;
    private MonsterBullet[] bulletComponent = new MonsterBullet[6];
    private Vector3 dir;
    private Vector3 newDir;
    private bool inAttack = false;
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

    public void LateUpdate()
    {
        if (inAttack)
        {
            if (inAttack) newDir = player.transform.position-transform.position;
            if(newDir != dir) 
            {
                dir = newDir;
                for (int i=0; i < 6; i++) bulletComponent[i].BulletInitialize(Quaternion.AngleAxis(Random.Range(-15, 15), Vector3.forward) * dir, 0f);
            }
        }
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
            dir = player.transform.position-transform.position;

            //Create Bullet
            for (int i=0; i < 6; i++)
            {
                bulletGo = MonsterPool.instance.pool.Get();
                bulletComponent[i] = bulletGo.GetComponent<MonsterBullet>();
                bulletGo.transform.position = transform.position;
                bulletComponent[i].BulletInitialize(Quaternion.AngleAxis(Random.Range(-15, 15), Vector3.forward) * dir, 0f);
            }

            Invoke("Go", 0f);
            inAttack = true;
        } 
        else if (loadedBullet <= 0)
        {
            Reload();
        }
    }

    private void Go()
    {
        inAttack = false;
        for (int i=0; i < 6; i++) bulletComponent[i].BulletInitialize(Quaternion.AngleAxis(Random.Range(-15, 15), Vector3.forward) * dir, Random.Range(3.0f, 7.0f));
    }

}