using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class BlanketInteraction : Interaction
{
    private Player player;
    private HwatuData[] hwatuData;

    public GameObject blanketUI;
    public BlanketHwatuButton[] buttons;
    Hwatu selectedHwatu;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        hwatuData = Resources.LoadAll<HwatuData>("HwatuData");
    }

    public override void LoadEvent()
    {
        Init();
    }

    private void Init()
    {
        isDone = false;
        blanketUI = UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[3];
        blanketUI.SetActive(true);

        buttons = blanketUI.GetComponentsInChildren<BlanketHwatuButton>();
        selectedHwatu = new Hwatu();

        InitButton();
    }

    private void InitButton()
    {
        int[] index = new int[3];
        while (!IsDuplicate(index))
        {
            index = GetRandomIndex();
        }

        for (int i=0; i<3; i++)
            buttons[i].SetButtonImage(hwatuData[index[i]]);

        buttons[0].button.onClick.AddListener(() => btnEvent(0));
        buttons[1].button.onClick.AddListener(() => btnEvent(1));
        buttons[2].button.onClick.AddListener(() => btnEvent(2));
    }

    private int[] GetRandomIndex()
    {
        int[] index = new int[3];
        for (int i = 0; i < 3; i++)
            index[i] = Random.Range(0, 20);
        return index;
    }

    private bool IsDuplicate(int[] index)
    {
        for(int i=0; i<3; i++)
        {
            List<Hwatu> usedHwatu = SkillManager.instance.hwatuCardSlotData; //이미 사용된 화투 
            for (int j = 0; j < usedHwatu.Count; j++) 
            {
                if (hwatuData[index[i]].hwatu == usedHwatu[j])
                    return false;
            }

            for(int j=i+1; j<3; j++) //3개 중 중복
            {
                if (index[i] == index[j])
                    return false;
            }
        }
        return true;
    }

    public void btnEvent(int selectedBtn)
    {
        for (int i = 0; i < 3; i++)
            buttons[i].button.onClick.RemoveAllListeners();

        selectedHwatu = buttons[selectedBtn].hwatu;
        SkillManager.instance.AddSkill(selectedHwatu);

        blanketUI.SetActive(false);
        isDone = true;
    }

}
