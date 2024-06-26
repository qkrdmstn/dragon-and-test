using System.Collections;
using System.Collections.Generic;
//using UnityEditor.ShaderKeywordFilter;
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

        else if(GameManager.instance.player.money < itemData.price)
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
                    //InventoryData.instance.AddItem(itemData);
                    if (GameManager.instance.player.curHP == GameManager.instance.player.maxHP) break;

                    GameManager.instance.player.curHP += 1;
                    break;
                case ItemType.Gun:
                    InventoryData.instance.AddGunItem(itemData);
                    break;
                case ItemType.Armor:
                    break;
            }
            GameManager.instance.player.money -= itemData.price;
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
