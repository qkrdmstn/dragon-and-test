using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private InventoryInfoUI info;

    public InventoryItem item;

    public void UpdateSlot(InventoryItem _newItem)
    {
        item = _newItem;
        itemImage.color = Color.white;

        if (item != null)
        {
            itemImage.sprite = item.data.icon;
            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();
            }
            else
            {
                itemText.text = "";
            }
        }
    }

    public void ClearSlot()
    {
        item = null;
        itemImage.sprite = null;
        itemText.text = "";
        itemImage.color = Color.red;

    }

    //Show description when clicking on item slot
    public void OnPointerDown(PointerEventData evenData)
    {
        if(item != null && item.data != null)
        {
            //Exception handling for click of dragonHwatu slot
            if (item.data.itemType == ItemType.DragonHwatu)
                return;

            info.UpdateItemInfo(item.data);
        }
    }
}
