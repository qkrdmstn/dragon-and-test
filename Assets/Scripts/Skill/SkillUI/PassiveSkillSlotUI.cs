using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class PassiveSkillSlotUI : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
{
    [Header("Skill Info")]
    public SeotdaHwatuCombination data;

    [Header("Info UI")]
    [SerializeField] private GameObject skillInfoUI;

    private void Awake()
    {
        data = SeotdaHwatuCombination.blank;
        skillInfoUI = transform.GetChild(0).gameObject;
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
