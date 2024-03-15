using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkill : MonoBehaviour
{
    #region Components
    private Player player;
    public Slider skillSlider;
    public RectTransform pass;
    #endregion

    [Header("Slider info")]
    public int sliderSpeed;

    [Header("Skill info")]
    public int drawCnt;
    public int successCnt;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && player.isCombatZone && !skillSlider.gameObject.activeSelf)
            InitializeSlider();
    }

    private void InitializeSlider()
    {
        drawCnt = 0;
        successCnt = 0;
        SetSlider();
        skillSlider.gameObject.SetActive(true);
    }

    private void SetSlider()
    {

        skillSlider.value = 0;
        drawCnt++;
        //image position random setting
        pass.anchoredPosition = new Vector2(Random.Range(0.0f, 180.0f), pass.anchoredPosition.y);
        StartCoroutine(DrawHwatu());
    }

    IEnumerator DrawHwatu()
    {
        yield return null;
        
        int dir = 1;
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            skillSlider.value += sliderSpeed * dir * Time.deltaTime;
            if (skillSlider.value >= skillSlider.maxValue || skillSlider.value <= skillSlider.minValue)
                dir *= -1;
            yield return null;
        }

        float minPos = pass.anchoredPosition.x;
        float maxPos = minPos + pass.sizeDelta.x;
        if (skillSlider.value >= minPos && skillSlider.value <= maxPos)
            successCnt++;

        if (drawCnt < 2)
            SetSlider();
        else
        {
            skillSlider.gameObject.SetActive(false);
            //Skill 효과 구현
            player.HP += successCnt;
        }
    }
}
