using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopRandomItem : MonoBehaviour
{
    [SerializeField] ItemObject[] itemObjs;

    public ItemData[] gunItemDatas;
    public ItemData[] armorItemDatas;
    public ItemData fruitItemData;

    int cnt = 0;

    private void Awake()
    {
        itemObjs = GetComponentsInChildren<ItemObject>();
        gunItemDatas = Resources.LoadAll<ItemData>("Gun/ItemData");
        armorItemDatas = Resources.LoadAll<ItemData>("Armor");
        fruitItemData = Resources.Load<ItemData>("Material/DragonFruit");
    }
    private void Start()
    {
        SetCountType();
    }
    // total Cnt = 4,  무기는 무조건 하나
    // case 1) 체력 2, 방어구 1
    // case 2) 체력 1, 방어구 2
    void SetCountType()
    {
        int armorCnt = Random.Range(1,3);
        int fruitCnt = 3 - armorCnt;
        SetCountGun();
        SetCountArmor(armorCnt);
        SetCountFruit(fruitCnt);
    }

    void SetCountGun()
    {
        int gunIdx = -1;
        do
        {
            gunIdx = Random.Range(0, 4);
        } while (gunIdx == 1);

        itemObjs[cnt].GetComponent<InteractionData>().itemData = itemObjs[cnt].itemData = gunItemDatas[gunIdx];
        itemObjs[cnt].SetItemData();
        cnt++;
    }

    void SetCountArmor(int _cnt)
    {
        itemObjs[cnt].GetComponent<InteractionData>().itemData = itemObjs[cnt].itemData = armorItemDatas[Random.Range(0, 2)];
        itemObjs[cnt].SetItemData();
        cnt++;
        if (_cnt == 2)
        {
            do
            {
                itemObjs[cnt].GetComponent<InteractionData>().itemData = itemObjs[cnt].itemData = armorItemDatas[Random.Range(0, 2)];
            } while (itemObjs[cnt-1].itemData == itemObjs[cnt].itemData);
            itemObjs[cnt].SetItemData();
            cnt++;
        }
    }

    void SetCountFruit(int _cnt)
    {
        itemObjs[cnt].GetComponent<InteractionData>().itemData = itemObjs[cnt].itemData = fruitItemData;
        itemObjs[cnt].SetItemData();
        cnt++;
        if(_cnt == 2) { 
            itemObjs[cnt].GetComponent<InteractionData>().itemData = itemObjs[cnt].itemData = fruitItemData;
            itemObjs[cnt].SetItemData();
        }
    }
}
