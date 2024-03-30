using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OldPlayerSkillHwatu : MonoBehaviour
{
    public HwatuCard card;
    public Month month;
    public HwatuType type;

    private float activeTimer;
    private float activeDuration = 0.8f;

    #region Components
    private OldPlayerSkill playerSkill;
    private Image image;
    #endregion

    private void Awake()
    {
        CardInit();
        playerSkill = GameObject.FindObjectOfType<OldPlayerSkill>();
        image = gameObject.GetComponent<Image>();
    }

    private void Update()
    {
        activeTimer -= Time.unscaledDeltaTime;
        image.color = new Color(1, 1, 1, 1.0f - (activeDuration - activeTimer) / (activeDuration * 1.5f));

        if (activeTimer < 0.0f)
            gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        playerSkill.curActiveCardNum++;
        activeTimer = activeDuration;
    }

    private void OnDisable()
    {
        playerSkill.curActiveCardNum--;

    }

    public void CardInit()
    {
        card.cardObj = this.gameObject;
        card.month = this.month;
        card.type = this.type;
        card.hwatu = this;
        card.rectTransform = this.GetComponent<RectTransform>();
    }
}
