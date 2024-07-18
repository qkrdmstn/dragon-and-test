using System.Collections;
using UnityEngine;

public enum StateOfBuy
{   // 0 : 의사결정중 1:구매 2:미구매 3:구매불가
    Nothing,
    YesBuy,
    NoBuy,
    CantBuy
};
public class ShopInteraction : Interaction
{
    public StateOfBuy state;

    GameObject interaction;
    ItemData itemData;
    public override void LoadEvent(InteractionData data)
    {
        Init();

        this.itemData = data.itemData;
        interaction = data.gameObject;

        StartCoroutine(ManageEvent());
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
        yield return new WaitUntil(() => state > 0);    // 의사결정완료

        if (state == StateOfBuy.YesBuy) {    // 구매시,
            switch (itemData.itemType)
            {
                case ItemType.Material:
                    if((itemData as EffectItemData) != null)
                        (itemData as EffectItemData).ItemEffect();
                    break;
                case ItemType.Gun:
                    InventoryData.instance.AddGunItem(itemData);
                    break;
                case ItemType.Armor:
                    (itemData as EffectItemData).ItemEffect();
                    InventoryData.instance.AddAmorItem(itemData);
                    break;
            }
            Player.instance.money -= itemData.price;
            Debug.Log(itemData.itemName + "Get Item!!");
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
