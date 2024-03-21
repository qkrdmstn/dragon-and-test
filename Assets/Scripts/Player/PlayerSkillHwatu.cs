using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillHwatu : MonoBehaviour
{
    private float activeTimer;
    private float activeDuration = 0.8f;

    #region Components
    private PlayerSkill playerSkill;
    private Image image;
    #endregion

    private void Awake()
    {
        playerSkill = GameObject.FindObjectOfType<PlayerSkill>();
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
}
