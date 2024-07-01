using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionData : MonoBehaviour
{
    public enum InteractionType
    {
        NPC,
        Item,
        Shop,
        Blanket,
        Tutorial
    };

    public InteractionType type;
    public string eventName;
    public int sequence;
    public Sprite npcImg;
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
