using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance = null;
    [Header("ItemData")]
    [HideInInspector] public ItemData[] gunItemDatas;
    [HideInInspector] public ItemData[] armorItemDatas;
    [HideInInspector]public ItemData fruitItemData;

    [Header("ItemPrefabs")]
    [HideInInspector]public GameObject fruitPrefab;
    [HideInInspector] public GameObject moneyPrefab;

    [Header("HwatuData")]
    [HideInInspector]public HwatuData[] hwatuDatas;      // all hwatu datas
    [HideInInspector]public GameObject[] hwatuUIObjs;    // all hwatu UI Objs
    [HideInInspector] public Dictionary<HwatuData, GameObject> hwatuDataUIObjs; // match for hwatuData to Objs

    public GameObject hwatuItemObj;
    int hwatuCardMaxCnt = 10;
    int hwatuCardCurCnt;
    public int refHwatuCardCnt
    {
        get { return hwatuCardCurCnt; }
        set
        {
            hwatuCardCurCnt = Mathf.Clamp(value, 0, hwatuCardMaxCnt);
            hwatuAction.Invoke();
        }
    }
    public Action hwatuAction;

    [HideInInspector] public GunController gunController;

    [Header("Holding")]
    public List<HwatuData> curHoldingHwatuDatas;

    void Awake()
    {
        if (instance == null)  {    // 생성 전이면
            instance = this;        // 생성
            //curHoldingGunItems = new Dictionary<GunItemData, GameObject>(); // save Data에서 init
            curHoldingHwatuDatas = new List<HwatuData>();

            LoadItemData();
            LoadItemPrefab();
            InitHwatus();
        }
        else if (instance != this)
        { //이미 생성되어 있으면
            Destroy(this.gameObject); // 새로만든거 삭제
        }
        DontDestroyOnLoad(this.gameObject); //씬이 넘어가도 오브젝트 유지
    }

    void Start()
    {
        gunController = GetComponentInChildren<GunController>();
    }

    #region Load & Init Datas
    void LoadItemData()
    {   
        gunItemDatas = Resources.LoadAll<ItemData>("Gun/ItemData");
        armorItemDatas = Resources.LoadAll<ItemData>("Armor");
        fruitItemData = Resources.Load<ItemData>("Material/DragonFruit");
    }

    void LoadItemPrefab()
    {
        fruitPrefab = Resources.Load<GameObject>("Prefabs/Item/Item Obj - DragonFruit");
        moneyPrefab = Resources.Load<GameObject>("Prefabs/Item/Money");

    }

    void InitHwatus()
    {
        hwatuDatas = Resources.LoadAll<HwatuData>("HwatuData");
        hwatuUIObjs = Resources.LoadAll<GameObject>("Prefabs/MaterialHwatuUI");
        foreach (GameObject item in hwatuUIObjs)
        {
            hwatuDataUIObjs.Add(item.GetComponent<MaterialHwatuSlotUI>().hwatuData, item);
        }
        refHwatuCardCnt = 0;
    }
    #endregion

    #region Hwatu
    public GameObject[] GetHoldingHwatuDataUIs()
    {
        List<GameObject> hwatuUIs = new List<GameObject>();
        foreach(HwatuData data in curHoldingHwatuDatas)
        {
            hwatuUIs.Add(hwatuDataUIObjs[data]);
        }
        return hwatuUIs.ToArray();
    }

    public void AddHwatuCard(HwatuData _data)
    {
        if (refHwatuCardCnt >= hwatuCardMaxCnt)
        {
            Debug.Log("The card data is full");
            return;
        }

        curHoldingHwatuDatas.Add(_data);
        refHwatuCardCnt = curHoldingHwatuDatas.Count;
        curHoldingHwatuDatas.Sort();
    }

    public void DeleteHwatuCard(HwatuData _data)
    {
        curHoldingHwatuDatas.Remove(_data);
        refHwatuCardCnt = curHoldingHwatuDatas.Count;
        curHoldingHwatuDatas.Sort();
    }

    public void DeleteAllHwatuCards()
    {
        curHoldingHwatuDatas.Clear();
        refHwatuCardCnt = curHoldingHwatuDatas.Count;
    }
    #endregion

    #region Gun
    public void LoadSaveGun(Dictionary<GunItemData, GameObject> saveDatas = null)
    {
        if(saveDatas == null)
        {
            gunController.AddGunData();
        }
        else
        {
            foreach(var data in saveDatas)
                gunController.gunAction(data.Key);
        }
    }
    #endregion
}
