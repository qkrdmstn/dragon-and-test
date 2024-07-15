using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class BlanketUI : MonoBehaviour
{
    [Header("Child UI")]
    public GameObject materialHwatuParent;
    public RectTransform blanket;
    public RectTransform trashCan;
    public Button exitButton;

    //public void Initialize()
    //{
    //    materialHwatuParent = transform.GetChild(0).gameObject;
    //}

    public bool IsInBlanket(RectTransform rectTransform)
    {
        Vector2 blanketMinPos = new Vector2(blanket.position.x, blanket.position.y)  - blanket.sizeDelta / 2;
        Vector2 blanketMaxPos = new Vector2(blanket.position.x, blanket.position.y)  + blanket.sizeDelta / 2;

        Vector2 targetMinPos = new Vector2(rectTransform.position.x, rectTransform.position.y) - rectTransform.sizeDelta / 2;
        Vector2 targetMaxPos = new Vector2(rectTransform.position.x, rectTransform.position.y) + rectTransform.sizeDelta / 2;

        if (blanketMinPos.x <= targetMinPos.x && blanketMinPos.y <= targetMinPos.y &&
            blanketMaxPos.x >= targetMaxPos.x && blanketMaxPos.y >= targetMaxPos.y)
            return true;
        else
            return false;
    }

    public bool IsInTrashCan(RectTransform rectTransform)
    {
        Vector2 trashCanMinPos = new Vector2(trashCan.position.x, trashCan.position.y) - trashCan.sizeDelta / 2;
        Vector2 trashCanMaxPos = new Vector2(trashCan.position.x, trashCan.position.y) + trashCan.sizeDelta / 2;

        Vector2 targetPos = new Vector2(rectTransform.position.x, rectTransform.position.y);

        if (trashCanMinPos.x <= targetPos.x && trashCanMinPos.y <= targetPos.y &&
            trashCanMaxPos.x >= targetPos.x && trashCanMaxPos.y >= targetPos.y)
            return true;
        else
            return false;
    }

    public void ExitBlanketInteraction()
    {
        BlanketInteraction blanketInteraction = FindObjectOfType<BlanketInteraction>();
        blanketInteraction.EndInteraction();
    }
}
