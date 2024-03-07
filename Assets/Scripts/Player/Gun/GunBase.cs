using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBase : MonoBehaviour
{
    #region Gun State
    protected bool isReloading = false;
    #endregion

    #region Gun Information
    public int damage = 1;
    public float shootDelay;
    public float reloadTime;

    public int maxBullet; //최대 총알 개수
    public int curTotalBullet; //현재 가진 총알
    public int magazineSize; //탄창 크기
    public int loadedBullet; //현재 장전된 총알
    #endregion

    public virtual void Shoot()
    {
        if(loadedBullet != 0)
        {
            Debug.Log("Base Shoot");
        }
    }

    public virtual void Reload()
    {
        isReloading = true;

    }
}
