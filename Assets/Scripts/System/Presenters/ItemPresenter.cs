using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class InventoryInformation
{
    public Image itemImg;
    public TextMeshProUGUI itemTitle, itemDesc;
}

public class ItemPresenter : PresenterBase
{
    [Header("Model")]
    public GunController m_Gun;
    public ItemManager m_Item;

    [Header("View")]
    public TextMeshProUGUI hwatuCnt;

    public Image gunImg;
    public Image reloadUIImg;

    public TextMeshProUGUI loadedBulletCnt;
    public TextMeshProUGUI maxBulletCnt;

    // Inventory Section
    public Transform[] gunInventory;
    public Transform armorInventory;

    public InventoryInformation itemInfoUI;

    #region ActionSetting
    void Awake()
    {
        m_Gun.gunAction += UpdateGunSlot;
        m_Gun.gunAction += UpdateBulletSlot;

        m_Gun.reloadAction += UpdateReloadUI;
        m_Gun.bulletAction += UpdateBulletSlot;
        m_Item.hwatuAction += HwatuCntChanged; // item presenter로 이전 필요
    }

    void Start()
    {
        m_Gun.addGunAction += UpdateGunInventory;
    }

    void OnDestroy()
    {
        m_Gun.gunAction -= UpdateGunSlot;
        m_Gun.gunAction -= UpdateBulletSlot;

        m_Gun.reloadAction -= UpdateReloadUI;
        m_Gun.bulletAction -= UpdateBulletSlot;
        m_Item.hwatuAction -= HwatuCntChanged;
    }
    #endregion

    #region Bullet
    void UpdateBulletSlot(GunItemData _gunData)
    {   
        UpdateLoadedBulletView(m_Gun.GetCurGunComponent().refLoadedBullet);
        UpdateMaxBulletView(m_Gun.GetCurGunComponent().refMaxBullet);
    }

    void UpdateBulletSlot(int _maxBullet, int _loadedBullet)
    {   // -1 is no update data
        if(_loadedBullet != -1)
            UpdateLoadedBulletView(_loadedBullet);
        if(_maxBullet != -1)
            UpdateMaxBulletView(_maxBullet);
    }

    void UpdateLoadedBulletView(int _loadedBullet)
    {
        loadedBulletCnt.text = "<size=36>" + _loadedBullet.ToString();
    }

    void UpdateMaxBulletView(int _maxBullet)
    {
        if (_maxBullet >= 10000) //총알 무한대
            maxBulletCnt.text = "</size><size=20> / ∞</size>";
        else
            maxBulletCnt.text = "</size><size=20> / " + _maxBullet.ToString() + "</size>";
    }
    #endregion


    #region Gun
    void UpdateGunSlot(GunItemData _newGun)
    {
        StopAllCoroutines(); // 재장전이라면 종료
        reloadUIImg.gameObject.SetActive(false);

        if (_newGun == null) ClearGunView();
        else UpdateGunView(_newGun);
    }

    void UpdateGunView(GunItemData _newGun)
    {
        gunImg.color = Color.white;
        gunImg.sprite = _newGun.icon;
    }

    void ClearGunView()
    {
        gunImg.sprite = null;
        gunImg.color = Color.red;
    }

    void UpdateReloadUI(bool on)
    {
        reloadUIImg.gameObject.SetActive(on);
        if (on)
        {
            StartCoroutine(ReloadProcess(m_Gun.GetCurGunComponent().reloadTime));
        }
    }

    IEnumerator ReloadProcess(float reloadTime)
    {
        float timer = 0.0f;
        while (timer <= reloadTime)
        {
            timer += Time.deltaTime;
            reloadUIImg.fillAmount = timer / reloadTime;

            //장전 UI 위치 설정
            Vector3 uiPos = Player.instance.transform.position + Vector3.up * 0.8f;
            reloadUIImg.rectTransform.position = Camera.main.WorldToScreenPoint(uiPos);
            yield return new WaitForFixedUpdate();
        }
        m_Gun.GetCurGunComponent().DoneReloadUI();
        reloadUIImg.gameObject.SetActive(false);
    }
    #endregion

    #region Inventory
    // Gun
    void UpdateGunInventory(GunItemData gunItemData)
    {

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
