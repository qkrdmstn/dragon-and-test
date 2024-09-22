using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryInfoUI : MonoBehaviour
{
    [SerializeField] Image infoImage;
    [SerializeField] TextMeshProUGUI infoItemName;
    [SerializeField] TextMeshProUGUI infoIteDesc;

    public void UpdateItemInfo(ItemData _data)
    {

        if (_data != null)
        {
            infoImage.color = Color.white;
            infoImage.sprite = _data.icon;
            infoItemName.text = _data.itemName;
            infoIteDesc.text = _data.itemInfo;
        }
    }
}
