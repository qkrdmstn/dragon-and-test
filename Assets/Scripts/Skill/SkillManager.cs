using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance = null;
    
    [Header("Hwatu Data")]
    public HwatuData[] hwatuData;

    [Header("DB")]
    [SerializeField] SkillBalanceTable skillTable;
    [SerializeField] SynergyInfo synergyTable;

    [Header("Skill Data")]
    public int skillCnt;
    public HwatuData[] hwatuCardSlotData;
    public SeotdaHwatuCombination curSynergy;
    public SkillBalanceEntity[] skillData;
    public float[] timer;

    [Header("Skill UI info")]
    [SerializeField] private GameObject overwritingUI;
    private CardOverwirteBtn[] overwriteBtns;
    [SerializeField] private Transform skillSlotParent;
    private SkillSlotUI[] skillSlot;
    [SerializeField] private SkillCoolTimeImg[] coolTimeImg;

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
            if (ScenesManager.instance.GetSceneNum() == (int)SceneInfo.Battle_1_A
                || ScenesManager.instance.GetSceneNum() == (int)SceneInfo.Battle_1_B
                || ScenesManager.instance.GetSceneNum() == (int)SceneInfo.Battle_1_C
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
        skillTable = Resources.Load<SkillBalanceTable>("SkillDB/SkillBalanceTable");
        for (int i=0; i<20; i++)
        {
            for(int j=0; j<20; j++)
            {
                if(skillTable.SkillTableEntity[i].cardName == hwatuData[j].hwatu.type.ToString())
                {
                    hwatuData[j].info = skillTable.SkillTableEntity[i].info;
                    break;
                }
            }
        }

        GameObject blanketUI = UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[3];
        overwritingUI = blanketUI.transform.GetChild(1).gameObject;
        overwriteBtns = overwritingUI.GetComponentsInChildren<CardOverwirteBtn>();

        curSynergy = SeotdaHwatuCombination.blank;

        skillSlotParent = UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[4].transform;
        skillSlot = skillSlotParent.GetComponentsInChildren<SkillSlotUI>();

        Transform cooltimeUIParent = skillSlotParent.GetChild(1);
        coolTimeImg = new SkillCoolTimeImg[2];
        coolTimeImg[0] = cooltimeUIParent.GetChild(0).GetComponent<SkillCoolTimeImg>();
        coolTimeImg[1] = cooltimeUIParent.GetChild(1).GetComponent<SkillCoolTimeImg>();

        timer = new float[2];

        hwatuCardSlotData = new HwatuData[2];
        skillData = new SkillBalanceEntity[2];
        skillCnt = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && !GameManager.instance.player.isInteraction) //Skill Slot Swap
        {
            SkillSwap();
        }

        if (Input.GetKeyDown(KeyCode.Q) && !GameManager.instance.player.isInteraction)
        {
            if (timer[0] > 0.0f)
                Debug.Log("Q Skill is cooldown time");
            else
                Skill(0);
        }

        if (Input.GetKeyDown(KeyCode.E) && !GameManager.instance.player.isInteraction)
        {
            if (timer[1] > 0.0f)
                Debug.Log("E Skill is cooldown time");
            else
                Skill(1);
        }
    }

    private void SkillSwap()
    {
        //Data Swap
        HwatuData temp = hwatuCardSlotData[0];
        hwatuCardSlotData[0] = hwatuCardSlotData[1];
        hwatuCardSlotData[1] = temp;
        UpdateSkillData();

        //CoolTime UI Swap
        float tempTimer = timer[0];
        timer[0] = timer[1];
        timer[1] = tempTimer;
        for (int i = 0; i < coolTimeImg.Length; i++)
        {
            if (skillData[i] == null)
                continue;
            coolTimeImg[i].gameObject.SetActive(timer[i] > 0.0f);
            StartCoroutine(CoolTimeFunc(skillData[i].coolTime, i, false));
        }
    }

    //Use Skill
    private void Skill(int i)
    {
        PlayerSkill skill = GameManager.instance.player.GetComponent<PlayerSkill>();
        SkillBalanceEntity data = skillData[i];
        if (hwatuCardSlotData[i] == null)
            return;
        skill.UseSkill(hwatuCardSlotData[i].hwatu.type, data.damage, data.range, data.force, data.speed);

        //CoolTime UI setting
        timer[i] = data.coolTime;
        coolTimeImg[i].gameObject.SetActive(true);
        StartCoroutine(CoolTimeFunc(data.coolTime, i, true));
    }

    //Skill Add on Slot
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
        UpdateSkillData();

         if (skillCnt == 2)
        {
            UpdateSynergy();
        }

    }

    private void UpdateSynergy()
    {
        curSynergy = Hwatu.GetHwatuCombination(hwatuCardSlotData[0].hwatu, hwatuCardSlotData[1].hwatu);
    }

    public SynergyEntity GetCurSynergyEntity()
    {
        for (int i = 0; i < synergyTable.SynergyEntity.Count; i++)
        {
            if (curSynergy.ToString() == synergyTable.SynergyEntity[i].synergyCode)
                return synergyTable.SynergyEntity[i];
        }
        return null;
    }

    public void OverwriteBtnEvent(int i, HwatuData data)
    {
        hwatuCardSlotData[i] = data;
        timer[i] = 0.0f;
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

    private void UpdateSkillData()
    {
        for(int i=0; i < 2; i++)
        {
            for(int j=0; j<skillTable.SkillTableEntity.Count; j++)
            {
                string cardName = skillTable.SkillTableEntity[j].cardName;
                SeotdaHwatuName type = (SeotdaHwatuName)Enum.Parse(typeof(SeotdaHwatuName), cardName);
                if (hwatuCardSlotData[i] == null)
                {
                    skillData[i] = null;
                    break;
                }
                else if (hwatuCardSlotData[i].hwatu.type == type)
                {
                    skillData[i] = skillTable.SkillTableEntity[j];
                }
            }
        }
        UpdateSkillSlot();
    }

    private void UpdateSkillSlot()
    {
        for(int i=0; i<2; i++)
            skillSlot[i].ClearSlot();
        for (int i = 0; i < 2; i++)
            skillSlot[i].UpdateSlot(hwatuCardSlotData[i]);
    }

    IEnumerator CoolTimeFunc(float coolTime, int i, bool flag)
    {
        if(flag)
            timer[i] = coolTime;
        while (timer[i] > 0.0f)
        {
            timer[i] -= Time.deltaTime;
            coolTimeImg[i].img.fillAmount = timer[i] / coolTime;
            yield return new WaitForFixedUpdate();
        }
        coolTimeImg[i].gameObject.SetActive(false);
    }
}
