using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using static UnityEditor.Progress;

public class PassiveSkillSlotUI : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
{
    [Header("Skill Info")]
    [SerializeField] private Image skillImage;
    [SerializeField] private TextMeshProUGUI skillInfoTxt;
    public SeotdaHwatuCombination data;

    private BlanketInteraction blanketInteraction;

    [Header("Info UI")]
    [SerializeField] private GameObject skillInfoUI;

    private void Awake()
    {
        data = SeotdaHwatuCombination.blank;
        skillInfoUI = transform.GetChild(0).gameObject;
    }

    private void Start()
    {
        blanketInteraction = FindObjectOfType<BlanketInteraction>();
    }

    public void UpdateSlot(SeotdaHwatuCombination skill)
    {
        data = skill;
        if (data != SeotdaHwatuCombination.blank)
        {
            skillImage.color = Color.white;
            skillImage.sprite = SkillManager.instance.skillSpriteDictionary[skill];
            skillInfoTxt.text = SkillManager.instance.GetSkillInfo(skill, true);
        }
        else 
        {
            skillImage.color = Color.clear;
            skillImage.sprite = null;
            skillInfoTxt.text = null;
        }
    }

    public void ClearSlot()
    {
        skillImage.sprite = null;
        skillImage.color = Color.clear;
        skillInfoTxt.text = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(data != SeotdaHwatuCombination.blank)
            skillInfoUI.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (data != SeotdaHwatuCombination.blank)
            skillInfoUI.SetActive(false);
    }
}
