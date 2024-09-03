using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public enum StateOfBuy
{   // 0 : 의사결정중 1:구매 2:미구매 3:구매불가
    Nothing,
    YesBuy,
    NoBuy,
    CantBuy
};
public class ShopInteraction : Interaction
{
    GameObject[] childUI;

    #region Item UI
    Image itemImg;
    TextMeshProUGUI itemName, itemInfo, itemPrice;
    #endregion

    #region Dialog UI
    TextMeshProUGUI dialogueTxt;
    List<TextMeshProUGUI> selectionTxt;
    #endregion

    public StateOfBuy state;
    public bool isFirst = true;
    public int result; // 선택된 답변의 배열 Idx

    GameObject interaction;
    ItemData itemData;
    public override void LoadEvent(InteractionData data)
    {
        Init();
        UIManager.instance.SceneUI["Shop"].SetActive(true);

        this.itemData = data.itemData;
        interaction = data.gameObject;

        if (isFirst) SetShopUI();
        UpdateItemData();

        StartCoroutine(ManageEvent());
    }

    void SetShopUI()
    {   // 관련 변수 할당
        childUI = UIManager.instance.SceneUI["Shop"].GetComponent<UIGroup>().childUI;

        // item
        itemImg = childUI[0].GetComponent<Image>();
        itemName = childUI[1].GetComponent<TextMeshProUGUI>();
        itemInfo = childUI[2].GetComponent<TextMeshProUGUI>();
        itemPrice = childUI[3].GetComponent<TextMeshProUGUI>();

        // dialog
        dialogueTxt = childUI[5].GetComponent<TextMeshProUGUI>();
        selectionTxt = new List<TextMeshProUGUI>
        {
            childUI[6].GetComponent<TextMeshProUGUI>(),
            childUI[7].GetComponent<TextMeshProUGUI>()
        };

        isFirst = false;
    }

    void UpdateDialogTxt()
    {   // 대화 string 등록
        // 구매의사 파악 

        // 구매 불가

        // 구매 가능

    }

    void UpdateItemData()
    {
        itemImg.sprite = itemData.icon;
        itemName.text = itemData.itemName;
        itemInfo.text = itemData.itemInfo;
        itemPrice.text = itemData.price.ToString();
    }

    public int CheckBuyState(int result)
    {
        if(result == 1) state = StateOfBuy.NoBuy;

        else if(Player.instance.money < itemData.price)
        {   // checkMoney
            state = StateOfBuy.CantBuy;
            return 2;
        }
        else state = StateOfBuy.YesBuy;

        return result;
    }

    IEnumerator ManageEvent()
    {
        yield return new WaitUntil(() => !isFirst);

        // TODO ------ 의사 결정 관련 함수

        yield return new WaitUntil(() => state > 0);    // 의사결정완료

        if (state == StateOfBuy.YesBuy) {    // 구매시,
            switch (itemData.itemType)
            {
                case ItemType.Material:
                    if((itemData as EffectItemData) != null)
                        (itemData as EffectItemData).ItemEffect();
                    break;
                case ItemType.Gun:
                    GunManager.instance.AddGun((itemData as GunItemData).gunData.gunPrefab);
                    break;
                case ItemType.Armor:
                    (itemData as EffectItemData).ItemEffect();
                    InventoryData.instance.AddArmorItem(itemData);
                    break;
            }
            Player.instance.money -= itemData.price;
            Destroy(interaction);
        }
    }

    void Init()
    {
        state = StateOfBuy.Nothing;
        interaction = null;
        itemData = null;
    }
}
