using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using static UnityEditor.PlayerSettings;

public class BlanketUI : MonoBehaviour
{
    [Header("Blanket UI")]
    public GameObject materialHwatuParent;
    public RectTransform blanket;
    public Button blanketExitButton;

    [Header("TrashCan UI")]
    public RectTransform trashCan;

    [Header("Skill Info UI")]
    public GameObject skillInfoUI;
    public Image skillImage;
    public TextMeshProUGUI skillNameTxt;
    public TextMeshProUGUI skillInfoTxt;
    public Button skillInfoExitButton;
    public bool isSkillInfoUI;

    [Header("Passive Skill Slot UI")]
    public Transform passiveSkillSlotParent;

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
        if (isSkillInfoUI) return false;
        else if (Player.instance.isTutorial)
        {
            FindObjectOfType<Tutorial>().OnTrashSkill();
            return false;
        }

        Vector2 trashCanMinPos = new Vector2(trashCan.position.x, trashCan.position.y) - trashCan.sizeDelta / 2;
        Vector2 trashCanMaxPos = new Vector2(trashCan.position.x, trashCan.position.y) + trashCan.sizeDelta / 2;

        Vector2 targetPos = new Vector2(rectTransform.position.x, rectTransform.position.y);

        if (trashCanMinPos.x <= targetPos.x && trashCanMinPos.y <= targetPos.y &&
            trashCanMaxPos.x >= targetPos.x && trashCanMaxPos.y >= targetPos.y)
            return true;
        else
            return false;
    }

    public void SetSkillInfoUIActive(SkillDB skillData, Sprite skillSprite) //스킬 조합 시, 호출되는 스킬 정보 UI 설정
    {
        skillImage.sprite = skillSprite;
        skillNameTxt.text = skillData.synergyName;
        skillInfoTxt.text = skillData.info;
        isSkillInfoUI = true;
        skillInfoUI.SetActive(true);

        Invoke("SetSkillInfoUIInActive", 3f);
    }

    public void SetSkillInfoUIInActive()
    {
        skillInfoUI.SetActive(false);
        isSkillInfoUI = false;

        Debug.Log("SkillInfo InActive");
    }

    public void ExitBlanketInteraction()
    {
        BlanketInteraction blanketInteraction = FindObjectOfType<BlanketInteraction>();
        blanketInteraction.EndInteraction();
    }
}
