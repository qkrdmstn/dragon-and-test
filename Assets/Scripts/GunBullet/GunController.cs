using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // ItemManager에서는 플레이어가 보유한 모든 총을 저장
    // 여기는 현재 총에 대한 정보만을 저장하고 행위 정의

    public GunItemData baseGunData;
    public Transform gunParent;
    public Dictionary<GunItemData, GameObject> curHoldingGunItems;
    public Action<GunItemData> gunAction;
    public Action uiAction;

    [Header("Swap info")]
    public GameObject currentGun;

    public int currentIdx;  // for add, swap index
    public int currentGunLoadedBullet;
    public int currentGunMaxBullet;

    public float swapDelay = 0.1f;
    public float swapTimer;

    void Awake()
    {
        curHoldingGunItems = new Dictionary<GunItemData, GameObject>();
    }

    void Start()
    {
        gunParent = Player.instance.transform.GetChild(1);
        currentIdx = 0;

        // 총 데이터 갱신 (gunController & inventory)
        gunAction += AddGunData;
        gunAction += InventoryData.instance.AddGunItem; // 수정예정
    }

    public void AddGunData()
    {   // 최초 총 세팅
        gunAction(baseGunData);
    }

    public void AddGunData(GunItemData itemData)
    {
        if (!curHoldingGunItems[itemData])
        {
            GameObject _newGunObj = Instantiate(itemData.gunPrefab, gunParent.position, gunParent.rotation, gunParent);
            curHoldingGunItems.Add(itemData, _newGunObj);

            _newGunObj.GetComponent<Gun>().initItemData = itemData;
            currentIdx = curHoldingGunItems.Count - 1;
        }
    }

    void SwapGun(bool up)
    {
        swapTimer = swapDelay;
        if (up)
        {
            currentIdx++;
            currentIdx %= curHoldingGunItems.Count;
        }
        else
        {
            currentIdx--;
            if (currentIdx < 0)
                currentIdx = curHoldingGunItems.Count - 1;
            currentIdx %= curHoldingGunItems.Count;
        }

        InitActiveGun();
    }

    public void DeleteGunData(GunItemData itemData)
    {
    }

    void InitActiveGun()
    {   // curGun 갱신하면서 프로퍼티 사용해서 이미지랑 총알 수 바인딩
        // 호출 정상화... 이것 또한... 은혜이겠지요
        currentGun.SetActive(false);
        currentGun = gunParent.GetChild(currentIdx).gameObject;
        currentGun.SetActive(true);

        //Gun Inventory Update
        //GunInventoryData.instance.UpdateGunInventorySlotUI(itemData); // gun image 갱신하는것 -> 이것도 gun presenter 연결
        //Gun _gun = currentGun.GetComponent<Gun>();
        //UpdateCurrentGunBulletData(_gun.maxBullet, _gun.loadedBullet);
    }

    //public void UpdateCurrentGunBulletData(int maxBullet, int loadedBullet)
    //{    // ui property - gun presenter
    //    currentGunMaxBullet = maxBullet; 
    //    currentGunLoadedBullet = loadedBullet;

    //    //GunInventoryData.instance.UpdateCurrentBulletUI(currentGunMaxBullet, currentGunLoadedBullet);
    //}
}
