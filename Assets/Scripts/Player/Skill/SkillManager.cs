using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance = null;
    
    [Header("Hwatu Data")]
    public HwatuData[] hwatuData;

    [Header("Skill Table DB")]
    [SerializeField] SkillBalanceTable skillTable;

    [Header("Skill Data")]
    public int skillCnt;
    public HwatuData[] hwatuCardSlotData;
    public SeotdaHwatuCombination curSynergy;
    public SkillBalanceEntity[] skillData;

    [Header("Skill UI info")]
    [SerializeField] private GameObject overwritingUI;
    private CardOverwirteBtn[] overwriteBtns;
    [SerializeField] private Transform skillSlotParent;
    private SkillSlotUI[] skillSlot;

    [Header("Skill State info")]
    public bool isSaving;
    public bool saveSuccess;

    private void Awake()
    {
        if (instance == null)
        { //생성 전이면
            instance = this; //생성
        }
        else if (instance != this)
        { //이미 생성되어 있으면
            if (ScenesManager.instance.GetSceneNum() == (int)SceneInfo.Battle_1
                || ScenesManager.instance.GetSceneNum() == (int)SceneInfo.Puzzle_1
                || ScenesManager.instance.GetSceneNum() == (int)SceneInfo.Boss_1)
            {
                Destroy(instance); //새로만든거 삭제
            }
            Destroy(this.gameObject); //새로만든거 삭제
        }

        DontDestroyOnLoad(this.gameObject); //씬이 넘어가도 오브젝트 유지
    }

    // Start is called before the first frame update
    void Start()
    {
        hwatuData = Resources.LoadAll<HwatuData>("HwatuData");

        GameObject blanketUI = UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[3];
        overwritingUI = blanketUI.transform.GetChild(1).gameObject;

        overwriteBtns = overwritingUI.GetComponentsInChildren<CardOverwirteBtn>();
        curSynergy = SeotdaHwatuCombination.blank;

        skillSlotParent = UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[4].transform;
        skillSlot = skillSlotParent.GetComponentsInChildren<SkillSlotUI>();

        hwatuCardSlotData = new HwatuData[2];
        skillData = new SkillBalanceEntity[2];
        skillCnt = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && !GameManager.instance.player.isInteraction) //Skill Slot Swap
        {
            HwatuData temp = hwatuCardSlotData[0];
            hwatuCardSlotData[0] = hwatuCardSlotData[1];
            hwatuCardSlotData[1] = temp;
            UpdateSkillData();
        }

        if(Input.GetKeyDown(KeyCode.Q) && !GameManager.instance.player.isInteraction)
            Skill(0);
        if (Input.GetKeyDown(KeyCode.E) && !GameManager.instance.player.isInteraction)
            Skill(1);
    }

    private void Skill(int i)
    {
        PlayerSkill skill = GameManager.instance.player.GetComponent<PlayerSkill>();
        SkillBalanceEntity data = skillData[i];
        if (hwatuCardSlotData[i] == null)
            return;
        skill.UseSkill(hwatuCardSlotData[i].hwatu.type, data.damage, data.range, data.force);
    }

    public void AddSkill(HwatuData data)
    {
        isSaving = true;
        saveSuccess = false;
        if (skillCnt < 2)
        {
            for(int i=0; i<2; i++)
            {
                if(hwatuCardSlotData[i] == null)
                {
                    hwatuCardSlotData[i] = data;
                    break;
                }
            }
            skillCnt++;
            isSaving = false;
            saveSuccess = true;
        }
        else
        {
            overwritingUI.SetActive(true);
            for (int i = 0; i < 2; i++)
            {
                int index = i;
                overwriteBtns[index].UpdateBtn(hwatuCardSlotData[index]);
                overwriteBtns[index].button.onClick.RemoveAllListeners();
                overwriteBtns[index].button.onClick.AddListener(() => OverwriteBtnEvent(index, data));
            }
        }
        
        if(skillCnt == 2)
            UpdateSynergy();
        UpdateSkillData();

        //Todo Update Combination Effect

    }

    public void UpdateSynergy()
    {
        curSynergy = Hwatu.GetHwatuCombination(hwatuCardSlotData[0].hwatu, hwatuCardSlotData[1].hwatu);
    }

    public void OverwriteBtnEvent(int i, HwatuData data)
    {
        hwatuCardSlotData[i] = data;
        saveSuccess = true;
        UpdateSynergy();
        InactiveOverwritingUI();
        UpdateSkillData();
    }

    public void InactiveOverwritingUI()
    {
        overwritingUI.SetActive(false);
        isSaving = false;
    }

    public void UpdateSkillData()
    {
        for(int i=0; i < 2; i++)
        {
            for(int j=0; j<skillTable.SkillTableEntity.Count; j++)
            {
                string cardName = skillTable.SkillTableEntity[j].cardName;
                SeotdaHwatuName type = (SeotdaHwatuName)Enum.Parse(typeof(SeotdaHwatuName), cardName);
                if (hwatuCardSlotData[i] == null)
                    continue;
                if (hwatuCardSlotData[i].hwatu.type == type)
                {
                    skillData[i] = skillTable.SkillTableEntity[j];
                }
            }
        }
        UpdateSkillSlot();
    }

    public void UpdateSkillSlot()
    {
        for(int i=0; i<2; i++)
        {
            skillSlot[i].ClearSlot();
        }
        for (int i = 0; i < 2; i++)
        {
            skillSlot[i].UpdateSlot(hwatuCardSlotData[i]);
        }
    }
}
