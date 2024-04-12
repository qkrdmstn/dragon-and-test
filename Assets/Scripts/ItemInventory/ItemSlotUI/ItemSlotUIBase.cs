using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlotUIBase : MonoBehaviour, IPointerDownHandler
{
    public virtual void UpdateSlot()
    {

    }

    public virtual void ClearSlot()
    {

    }

    //Show description when clicking on item slot
    public virtual void OnPointerDown(PointerEventData evenData)
    {

    }

}
