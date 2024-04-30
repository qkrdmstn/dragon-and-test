using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionData : MonoBehaviour
{
    public enum InteractionType
    {
        NPC,
        Item
    };

    public InteractionType type;
    public string eventName;
    public ItemData itemData;
    private void OnValidate()
    {
        if (itemData!=null)
        {
            GetComponent<SpriteRenderer>().sprite = itemData.icon;
            gameObject.name = "Item Obj - " + itemData.itemName;
        }
    }
}
