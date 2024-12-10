using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ItemPresenter : PresenterBase
{
    [Header("Model")]
    public GunController m_Gun;
    public ItemManager m_Item;

    [Header("View")]
    public TextMeshProUGUI hwatuCnt;

    public Image gunImg;
    public InventoryItem curGun;

    public TextMeshProUGUI bulletCnt;

    #region ActionSetting
    void Start()
    {
        m_Gun.uiAction += UpdateBulletSlot;
        m_Item.hwatuAction += HwatuCntChanged; // item presenter로 이전 필요
    }

    void OnDestroy()
    {
        m_Gun.uiAction -= UpdateBulletSlot;
        m_Item.hwatuAction -= HwatuCntChanged;
    }
    #endregion

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
        if (_newGun == null) ClearGunView();
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

    #region Hwatu
    public void HwatuCntChanged()
    {
        UpdateHwatuTxtCnt();
    }

    void UpdateHwatuTxtCnt()
    {
        hwatuCnt.text = "X " + m_Item.refHwatuCardCnt.ToString();
    }
    #endregion
}
