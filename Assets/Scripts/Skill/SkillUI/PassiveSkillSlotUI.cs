using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using static UnityEditor.Progress;

public class PassiveSkillSlotUI : MonoBehaviour
{
    [Header("Skill Info")]
    [SerializeField] private Image skillImage;
    public SeotdaHwatuCombination data;

    private BlanketInteraction blanketInteraction;

    private void Awake()
    {
        skillImage = GetComponent<Image>();

        data = SeotdaHwatuCombination.blank;
    }

    private void Start()
    {
        blanketInteraction = FindObjectOfType<BlanketInteraction>();
    }

    public void UpdateSlot(SeotdaHwatuCombination _data)
    {
        data = _data;
        skillImage.color = Color.white;

        if (data != SeotdaHwatuCombination.blank)
            skillImage.sprite = SkillManager.instance.skillSpriteDictionary[_data];
    }

    public void ClearSlot()
    {
        skillImage.sprite = null;
        skillImage.color = Color.white;
    }
}
