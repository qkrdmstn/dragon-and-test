using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopRandomItem : MonoBehaviour
{
    [SerializeField] ItemObject[] itemObjs;
    int cnt = 0;

    private void Awake()
    {
        itemObjs = GetComponentsInChildren<ItemObject>();
    }
    private void Start()
    {
        SetCountType();
    }
    // total Cnt = 4,  무기는 무조건 하나
    // case 1) 체력 2, 방어구 1
    // case 2) 체력 1, 방어구 2
    void SetCountType()
    {   // 26/01/15기준 case1만 진행
        int armorCnt = 1; //Random.Range(1,3);
        int fruitCnt = 2; //3 - armorCnt;
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

        itemObjs[cnt].GetComponent<InteractionData>().itemData = itemObjs[cnt].itemData = ItemManager.instance.gunItemDatas[gunIdx];
        itemObjs[cnt].SetItemData();
        cnt++;
    }

    void SetCountArmor(int _cnt)
    {
        itemObjs[cnt].GetComponent<InteractionData>().itemData = itemObjs[cnt].itemData = ItemManager.instance.armorItemDatas[Random.Range(0, 1)]; // 1월 방어구만 나오게 변경
        itemObjs[cnt].SetItemData();
        cnt++;
        if (_cnt == 2)
        {
            do
            {
                itemObjs[cnt].GetComponent<InteractionData>().itemData = itemObjs[cnt].itemData = ItemManager.instance.armorItemDatas[Random.Range(0, 2)];
            } while (itemObjs[cnt-1].itemData == itemObjs[cnt].itemData);
            itemObjs[cnt].SetItemData();
            cnt++;
        }
    }

    void SetCountFruit(int _cnt)
    {
        itemObjs[cnt].GetComponent<InteractionData>().itemData = itemObjs[cnt].itemData = ItemManager.instance.fruitItemData;
        itemObjs[cnt].SetItemData();
        cnt++;
        if(_cnt == 2) { 
            itemObjs[cnt].GetComponent<InteractionData>().itemData = itemObjs[cnt].itemData = ItemManager.instance.fruitItemData;
            itemObjs[cnt].SetItemData();
        }
    }
}
