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

    public void UpdateSlot(SeotdaHwatuCombination _data)
    {
        data = _data;
        skillImage.color = Color.white;
        skillInfoTxt.text = SkillManager.instance.skillDBDictionary[_data].info;

        if (data != SeotdaHwatuCombination.blank)
            skillImage.sprite = SkillManager.instance.skillSpriteDictionary[_data];
    }

    public void ClearSlot()
    {
        skillImage.sprite = null;
        skillImage.color = Color.white;
        skillInfoTxt.text = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        skillInfoUI.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        skillInfoUI.SetActive(false);
    }
}
