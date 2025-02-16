using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IInventory
{
    public void SetInfoUI(ItemData itemData);
    public void ResetInfoUI();
}

[System.Serializable]
public class InventoryInformation : IInventory
{
    public Image itemImg;
    public TextMeshProUGUI itemTitle, itemDesc;

    public Sprite originImg;
    bool isFirst = true;

    public void ResetInfoUI()
    {
        if (isFirst)
        {
            originImg = itemImg.sprite;
            isFirst = false;
        }
        else itemImg.sprite = originImg;
        itemTitle.text = "";
        itemDesc.text = "";
    }

    public void SetInfoUI(ItemData itemData)
    {
        itemImg.sprite = itemData.icon;
        itemTitle.text = itemData.itemName;
        itemDesc.text = itemData.itemInfo;
    }
}

[System.Serializable]
public class ItemInventory
{
    public GameObject itemSlot;
    ItemData itemData;
    Image itemImg;
    Sprite noneItemImg;

    TextMeshProUGUI itemCnt;
    IInventory inventory; // 인터페이스로 OnPointerDown 함수를 통한 인터페이스 함수 호출

    public void Init(IInventory inventory)
    {
        itemImg = itemSlot.GetComponent<Image>();
        noneItemImg = itemImg.sprite;
        this.inventory = inventory;

        itemCnt = itemSlot.GetComponentInChildren<TextMeshProUGUI>();
        SetEventTrigger();
    }

    public void SetItemData(ItemData itemData)
    {
        this.itemData = itemData;
        itemImg.sprite = itemData.icon;
        itemCnt.text = "";
    }

    void SetEventTrigger()
    {    // 개별 아이템 슬롯 세팅
        EventTrigger eventTrigger = itemSlot.AddComponent<EventTrigger>();

        EventTrigger.Entry entry_PointerDown = new EventTrigger.Entry();
        entry_PointerDown.eventID = EventTriggerType.PointerDown;
        entry_PointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerDown);
    }

    void OnPointerDown(PointerEventData evenData)
    {
        if(itemData != null)
        {
            inventory.SetInfoUI(itemData);
            SoundManager.instance.PlayUIEffeect(5); // click
        }
    }

    public void DeleteItem()
    {
        itemImg.sprite = noneItemImg;
        itemData = null;

        inventory?.ResetInfoUI();
    }
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
    public InventoryInformation itemInfoUI;
    public List<ItemInventory> gunInventory;
    int curGunIdx = 0;

    public ItemInventory armorInventory;

    #region ActionSetting
    void Awake()
    {
        //gunInventory = new List<ItemInventory> ();

        m_Gun.gunAction += UpdateGunSlot;
        m_Gun.gunAction += UpdateBulletSlot;

        m_Gun.reloadAction += UpdateReloadUI;
        m_Gun.bulletAction += UpdateBulletSlot;

        m_Item.hwatuAction += HwatuCntChanged;
        m_Item.armorAction += UpdateArmorInventory;
    }

    void Start()
    {
        m_Gun.addGunAction += UpdateGunInventory;

        InitInventoryDatas();
    }

    void InitInventoryDatas()
    {
        foreach(ItemInventory data in gunInventory)
        {
            data.Init(itemInfoUI);
        }
        armorInventory.Init(itemInfoUI);
    }

    void OnDestroy()
    {
        m_Gun.gunAction -= UpdateGunSlot;
        m_Gun.gunAction -= UpdateBulletSlot;

        m_Gun.reloadAction -= UpdateReloadUI;
        m_Gun.bulletAction -= UpdateBulletSlot;

        m_Item.hwatuAction -= HwatuCntChanged;
        m_Item.armorAction -= UpdateArmorInventory;
    }
    #endregion

    #region Bullet
    void UpdateBulletSlot(GunItemData _gunData)
    {   
        if(_gunData == null)
        {
            UpdateLoadedBulletView(0);
            UpdateLoadedBulletView(0);
        }
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

        if (_newGun == null)
        {
            ClearGunView();
            ClearGunInventory();
        }
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
    {   // 이미 중복에 대한 체크가 되고 액션 호출
        if (curGunIdx > 3) return;
        gunInventory[curGunIdx++].SetItemData(gunItemData);
    }
    void ClearGunInventory()
    {   // 사망에 따른 모든 총 초기화 작업
        while (curGunIdx >= 0)
        {
            gunInventory[curGunIdx--].DeleteItem();
        }
        curGunIdx = 0;
    }

    // Armor
    void UpdateArmorInventory(ItemData armorItemData)
    {   // 방어구가 대체되는 구조 - 하나만 보유 가능
        if(armorItemData == null)
        {   // null이라면 삭제의 경우
            armorInventory.DeleteItem();
        }
        else armorInventory.SetItemData(armorItemData);
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

    public override SceneInfo ActivateEachUI()
    {
        if (base.ActivateEachUI() == SceneInfo.Battle_1_A)
        {   // UI기 켜져야하는 배틀, 튜토리얼, 보스, 퍼즐씬
            objs[1].SetActive(true);    // gun
            objs[2].SetActive(true);    // bullet
            objs[3].SetActive(true);    // inventory
        }
        else if(base.ActivateEachUI() == SceneInfo.Town_1)
        {
            objs[3].SetActive(true);    // inventory
        }
        return SceneInfo.Battle_1_A;
    }
}
