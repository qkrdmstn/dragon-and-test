using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GunInventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;

    public InventoryItem item;

    public void UpdateSlot(InventoryItem _newItem)
    {
        item = _newItem;
        itemImage.color = Color.white;

        if (item != null)
        {
            itemImage.sprite = item.data.icon;
        }
    }

    public void ClearSlot()
    {
        item = null;
        itemImage.sprite = null;
        itemImage.color = Color.red;
    }
}
