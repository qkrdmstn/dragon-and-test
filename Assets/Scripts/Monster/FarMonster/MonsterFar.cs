using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFar : MonsterBase
{
    #region MonsterShoot
    public float monsterShootTimer;
    public float monsterShootDelay = 0.7f;
    public float monsterReloadDelay = 2f;
    public bool isReloading = false;
    public int loadedBullet;
    public int magazineSize=3;
    public GameObject monsterBullet;
    #endregion

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
        
        //Attack
        monsterShootTimer -= Time.deltaTime;
        if(!isReloading && !isKnockedBack && Vector3.Distance(transform.position, player.transform.position)<50.0f) Attack();
    }
    
    public override void Attack()
    {
        Shoot();
    }
    
    public void Shoot()
    {
        if(loadedBullet > 0 && monsterShootTimer < 0.0)
        {
            monsterShootTimer = monsterShootDelay;
            loadedBullet--;

            //Create Bullet
            var bulletGo = MonsterPool.instance.pool.Get();
            var bulletComponent = bulletGo.GetComponent<MonsterBullet>();
            bulletGo.transform.position = transform.position;
            bulletComponent.BulletInitialize(player.transform.position-transform.position);

        } 
        else if (loadedBullet <= 0)
        {
            Reload();
        }
    }

    public void Reload()
    {
        isReloading = true;
        StartCoroutine(ReloadProcess());
    }

    IEnumerator ReloadProcess()
    {
        yield return new WaitForSeconds(monsterReloadDelay);
        loadedBullet = magazineSize;
        isReloading = false;
    }

}