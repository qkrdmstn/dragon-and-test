using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance = null;
    [Header("ItemData")]
    public ItemData[] gunItemDatas;
    public ItemData[] armorItemDatas;
    public ItemData fruitItemData;

    [Header("ItemPrefabs")]

    public Dictionary<GunItemData, GameObject> curHoldingGunItems;

    void Awake()
    {
        if (instance == null)
        { //생성 전이면
            instance = this; //생성
            curHoldingGunItems = new Dictionary<GunItemData, GameObject>();
            LoadItemData();
        }
        else if (instance != this)
        { //이미 생성되어 있으면
            Destroy(this.gameObject); //새로만든거 삭제
        }

        DontDestroyOnLoad(this.gameObject); //씬이 넘어가도 오브젝트 유지
    }

    void LoadItemData()
    {   
        gunItemDatas = Resources.LoadAll<ItemData>("Gun/ItemData");
        armorItemDatas = Resources.LoadAll<ItemData>("Armor");
        fruitItemData = Resources.Load<ItemData>("Material/DragonFruit");
    }
}
