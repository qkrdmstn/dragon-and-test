using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class PickUpItemInteraction : Interaction
{

    GameObject interaction;
    ItemData itemData;

    public override void LoadEvent(InteractionData data)
    {
        Init();

        this.itemData = data.itemData;
        interaction = data.gameObject;

        switch (itemData.itemType)
        {
            case ItemType.Material:
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
        isDone = true;
        Destroy(interaction);
    }

    void Init()
    {
        isDone = false;
        interaction = null;
        itemData = null;
    }
}
