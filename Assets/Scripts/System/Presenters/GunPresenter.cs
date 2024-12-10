using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunPresenter : PresenterBase
{
    [Header("Model")]
    public GunManager m_Gun;

    [Header("View")]
    public Image gunImg;
    public InventoryItem curGun;

    public TextMeshProUGUI bulletCnt;

    private void Start()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    #region Bullet
    void UpdateBulletSlot()
    {

    }

    void UpdateBulletView(int _maxBullet, int _loadedBullet)
    {
        if (_maxBullet >= 10000) //총알 무한대
            bulletCnt.text = "<size=36>" + _loadedBullet.ToString() + "</size><size=20> / ∞</size>";
        else
            bulletCnt.text = "<size=36>" + _loadedBullet.ToString() + "</size><size=20> / " + _maxBullet.ToString() + "</size>";
    }
    #endregion


    #region Gun
    void UpdateGunSlot(InventoryItem _newGun)
    {
        if(_newGun == null) ClearGunView();
        else UpdateGunView(_newGun);
    }

    void UpdateGunView(InventoryItem _newGun)
    {
        curGun = _newGun;
        gunImg.color = Color.white;
        gunImg.sprite = curGun.data.icon;
    }

    void ClearGunView()
    {
        curGun = null;
        gunImg.sprite = null;
        gunImg.color = Color.red;
    }
    #endregion
}
