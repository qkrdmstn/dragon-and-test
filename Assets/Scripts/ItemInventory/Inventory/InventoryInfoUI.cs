using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryInfoUI : MonoBehaviour
{
    [SerializeField] private Image infoImage;
    [SerializeField] private TextMeshProUGUI infoText;

    public void UpdateItemInfo(ItemData _data)
    {

        if (_data != null)
        {
            infoImage.color = Color.white;
            infoImage.sprite = _data.icon;
            infoText.text = _data.itemInfo;
        }
    }
}
