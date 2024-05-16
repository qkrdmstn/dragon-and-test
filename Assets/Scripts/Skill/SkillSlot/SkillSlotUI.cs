using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEditor.Progress;

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private Image skillImage;
    public HwatuData data;
    private void Awake()
    {
        skillImage = GetComponent<Image>();
    }

    public void UpdateSlot(HwatuData _data)
    {
        data = _data;
        skillImage.color = Color.white;

        if (data != null)
        {
            skillImage.sprite = data.sprite;
        }
    }

    public void ClearSlot()
    {
        skillImage.sprite = null;
        skillImage.color = Color.red;
    }
}
