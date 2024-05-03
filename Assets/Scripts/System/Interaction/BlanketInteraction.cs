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
    public GameObject blanketUI;
    public BlanketHwatuButton[] buttons;

    public int selectCnt = 0;
    public int selectedIdx1 = -1;
    Hwatu[] selectedHwatu;

    public override void LoadEvent()
    {
        Init();
    }

    private void Init()
    {
        isDone = false;
        blanketUI = UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[3];
        blanketUI.SetActive(true);
        selectCnt = 0;
        selectedIdx1 = -1;

        buttons = blanketUI.GetComponentsInChildren<BlanketHwatuButton>();
        selectedHwatu = new Hwatu[2];

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
            buttons[i].SetButtonImage(index[i]);

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
            if (index[i] == selectedIdx1)
                return false;
            for(int j=i+1; j<3; j++)
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

        if (selectedIdx1 == -1) // 첫 번째 선택
        {
            selectedHwatu[selectCnt] = buttons[selectedBtn].hwatu;
            selectedIdx1 = buttons[selectedBtn].hwatuIdx;
            selectCnt++;
            InitButton();

        }
        else
        {
            selectedHwatu[selectCnt] = buttons[selectedBtn].hwatu;
            SeotdaHwatuCombination result = Hwatu.GetHwatuCombination(selectedHwatu[0], selectedHwatu[1]);
            Debug.Log(result);
            
            blanketUI.SetActive(false);
            isDone = true;
        }
    }
}
