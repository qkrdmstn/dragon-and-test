using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Hardware;
using UnityEditor.Experimental.GraphView;

public class BlanketInteraction : Interaction
{
    private Player player;

    [Header("UI info")]
    [SerializeField] private GameObject blanketUI;
    public BlanketHwatuSelectBtn[] selectBtns;
    [SerializeField] private GameObject cancleUI;
    public Button[] cancleBtns;
    [SerializeField] private SynergyUI synergyUI;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && player.isCombatZone && !isDone && player.isInteraction)
        {
            if(synergyUI.gameObject.activeSelf)
                synergyUI.gameObject.SetActive(false);
            else if (SkillManager.instance.isSaving)
                SkillManager.instance.InactiveOverwritingUI();
            else
            {
                ActiveCancleUI();
            }
        }
    }

    public override void LoadEvent()
    {
        Init();
    }

    private void Init()
    {
        isDone = false;
        blanketUI = UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[3];
        selectBtns = blanketUI.GetComponentsInChildren<BlanketHwatuSelectBtn>();

        cancleUI = blanketUI.transform.GetChild(2).gameObject;
        cancleBtns = cancleUI.GetComponentsInChildren<Button>();

        synergyUI = blanketUI.transform.GetChild(3).GetComponent<SynergyUI>();

        blanketUI.SetActive(true);
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
        {
            HwatuData data = SkillManager.instance.hwatuData[index[i]];
            selectBtns[i].UpdateBtn(data);
            selectBtns[i].button.onClick.RemoveAllListeners();
            selectBtns[i].button.onClick.AddListener(() => SelectBtnEvent(data));
        }
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
            HwatuData[] usedHwatu = SkillManager.instance.hwatuCardSlotData; //이미 사용된 화투 
            for (int j = 0; j < 2; j++) 
            {
                if (usedHwatu[j] == null)
                    continue;
                if (SkillManager.instance.hwatuData[index[i]].hwatu == usedHwatu[j].hwatu)
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

    public void SelectBtnEvent(HwatuData data)
    {
        
        SkillManager.instance.AddSkill(data);

        StartCoroutine(WaitOverwriting());
    }

    IEnumerator WaitOverwriting()
    {
        yield return new WaitUntil(() => !SkillManager.instance.isSaving && SkillManager.instance.saveSuccess);
        InactiveUI(false);
    }

    public void CancleBtnEvent(int i)
    {
        if (i == 0)
            InactiveUI(true);
        cancleUI.SetActive(false);
    }

    private void ActiveCancleUI()
    {
        cancleUI.SetActive(true);
        for (int i = 0; i < 2; i++)
        {
            int index = i;
            cancleBtns[index].onClick.RemoveAllListeners();
            cancleBtns[index].onClick.AddListener(() => CancleBtnEvent(index));
        }
    }

    private void InactiveUI(bool isCancle)
    {
        if (SkillManager.instance.skillCnt == 2 && !isCancle) //스킬 획득 포기 시, SynergyUI Active X
            StartCoroutine(ActiveSynergyUI());
        else
        {
            blanketUI.SetActive(false);
            isDone = true;
        }
    }

    IEnumerator ActiveSynergyUI()
    {
        synergyUI.gameObject.SetActive(true);
        SynergyEntity entity = SkillManager.instance.GetCurSynergyEntity();
        HwatuData[] hwatuDatas = SkillManager.instance.hwatuCardSlotData;
        synergyUI.UpdateSynergyUI(entity, hwatuDatas);

        yield return new WaitUntil(() => !synergyUI.gameObject.activeSelf);
        blanketUI.SetActive(false);
        isDone = true;
    }
}
