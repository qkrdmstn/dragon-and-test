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

    public int maxBullet; //�ִ� �Ѿ� ����
    public int curTotalBullet; //���� ���� �Ѿ�
    public int magazineSize; //źâ ũ��
    public int loadedBullet; //���� ������ �Ѿ�
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
