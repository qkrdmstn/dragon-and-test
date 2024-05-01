using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public enum StateOfBuy
{   // 0 : 의사결정중 1:구매 2:미구매
    Nothing,
    YesBuy,
    NoBuy
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

    IEnumerator ManageEvent()
    {
        yield return new WaitUntil(() => state > 0);    // 의사결정완료

        if (state == StateOfBuy.YesBuy) {    // 구매시,
            Debug.Log(itemData.itemName + "Get Item!!");
            InventoryData.instance.AddItem(itemData);
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
