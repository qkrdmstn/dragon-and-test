using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance = null;
    
    [Header("Hwatu Data")]
    public HwatuData[] hwatuData; //전체 카드 데이터
    public GameObject hwatuItemObj;
    public List<HwatuData> materialHwatuDataList; //조합 카드 데이터
    public int materialCardCnt = 0;
    public int materialCardMaxNum = 10;

    [Header("DB")]
    SkillDB[] datas;
    public Dictionary<SeotdaHwatuCombination, SkillDB> skillDBDictionary = new Dictionary<SeotdaHwatuCombination, SkillDB>();
    public Dictionary<SeotdaHwatuCombination, Sprite> skillSpriteDictionary = new Dictionary<SeotdaHwatuCombination, Sprite>();

    [Header("Skill Data")]
    //Active Skill
    public int activeSkillCnt;
    public SeotdaHwatuCombination[] activeSkillData;
    public float[] timer;

    //Passive Skill
    public Dictionary<SeotdaHwatuCombination, int> passiveSkillCnt = new Dictionary<SeotdaHwatuCombination, int>();
    public List<SeotdaHwatuCombination> passiveSkillData;

    [Header("Skill UI info")]
    [SerializeField] private Transform activeSkillSlotParent;
    public ActiveSkillSlotUI[] activeSkillSlot;
    [SerializeField] private SkillCoolTimeImg[] coolTimeImg;
    [SerializeField] private Transform passiveSkillSlotParent;
    public PassiveSkillSlotUI[] passiveSkillSlot;

    private void Awake()
    {
        if (instance == null)
        { //생성 전이면
            instance = this; //생성
        }
        else if (instance != this)
        { //이미 생성되어 있으면

            Destroy(this.gameObject); //새로만든거 삭제
        }

        DontDestroyOnLoad(this.gameObject); //씬이 넘어가도 오브젝트 유지
    }

    async void Start()
    {   //Initialize
        InitializeSkillData();
        InitializeUI(); 
        await LoadSkillDB();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !Player.instance.isInteraction) //Skill Slot Swap
        {
            SkillSwap();
        }

        if (Input.GetKeyDown(KeyCode.Q) && !Player.instance.isInteraction)
        {
            if (timer[0] > 0.0f)
                Debug.Log("Q Skill is cooldown time");
            else
                Skill(0);
        }

        if (Input.GetKeyDown(KeyCode.E) && !Player.instance.isInteraction)
        {
            if (timer[1] > 0.0f)
                Debug.Log("E Skill is cooldown time");
            else
                Skill(1);
        }
    }

    #region Initialize Func
    private void InitializeSkillData()
    {
        //화투 데이터 로드
        hwatuData = Resources.LoadAll<HwatuData>("HwatuData");

        //스킬 이미지 로드, Dictionary 구성
        Sprite [] skillImages = Resources.LoadAll<Sprite>("SkillSprite");
        for(int i = 0; i < skillImages.Length; i++)
        {
            for (int j = 0; j < 33; j++)
            {
                if(((SeotdaHwatuCombination)j).ToString() == skillImages[i].name)
                {
                    skillSpriteDictionary.Add((SeotdaHwatuCombination)j, skillImages[i]);
                    break;
                }
            }
        }

        materialHwatuDataList = new List<HwatuData>();
        passiveSkillData = new List<SeotdaHwatuCombination>();
        activeSkillData = new SeotdaHwatuCombination[]{ SeotdaHwatuCombination.blank, SeotdaHwatuCombination.blank };
        activeSkillCnt = 0;
        timer = new float[2];
    }

    private async Task LoadSkillDB()
    {
        //스킬 DataBase 로드
        datas = await DataManager.instance.GetValues<SkillDB>(SheetType.SkillDB, "A1:J33");
        for (int i = 0; i < datas.Length; i++)
        {
            skillDBDictionary.Add(datas[i].TransStringToEnum(), datas[i]);
        }
        ScenesManager.instance.isLoadedDB++;
    }

    private void InitializeUI()
    {
        activeSkillSlotParent = UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[3].transform;
        activeSkillSlot = activeSkillSlotParent.GetComponentsInChildren<ActiveSkillSlotUI>();

        passiveSkillSlotParent = UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[5].GetComponent<BlanketUI>().passiveSkillSlotParent;
        passiveSkillSlot = passiveSkillSlotParent.GetComponentsInChildren<PassiveSkillSlotUI>();

        Transform cooltimeUIParent = activeSkillSlotParent.GetChild(1);
        coolTimeImg = new SkillCoolTimeImg[2];
        for (int i = 0; i < 2; i++)
        {
            activeSkillSlot[i].ClearSlot();
            coolTimeImg[i] = cooltimeUIParent.GetChild(i).GetComponent<SkillCoolTimeImg>();
            coolTimeImg[i].gameObject.SetActive(false);
        }
    }
    #endregion

    public void AddMaterialCardData(HwatuData _data)
    {
        if(materialCardCnt >= materialCardMaxNum)
        {
            Debug.Log("The card data is full");
            return;
        }    

        materialHwatuDataList.Add(_data);
        materialCardCnt = materialHwatuDataList.Count;
        materialHwatuDataList.Sort();
    }

    public void DeleteMaterialCardData(HwatuData _data)
    {
        materialHwatuDataList.Remove(_data);
        materialCardCnt = materialHwatuDataList.Count;
        materialHwatuDataList.Sort();
    }

    public void DeleteAllCardData()
    {
        materialHwatuDataList.Clear();
        materialCardCnt = materialHwatuDataList.Count;
    }

    public void AddSkill(SeotdaHwatuCombination skill)
    {
        int skillNum = (int)skill;
        //Add Passive Skill
        if (skillNum >= 13 && skillNum <= 18)
        {
            //스킬 보유 X
            if (passiveSkillData.FindIndex(x => x == skill) == -1)
            {
                passiveSkillData.Add(skill);
                passiveSkillCnt.Add(skill, 0);
            }
            passiveSkillCnt[skill]++;

            UpdatePassiveSkillSlot();
        }
        else //Add Active Skill
        {
            if (activeSkillData[0] == SeotdaHwatuCombination.blank)
                activeSkillData[0] = skill;
            else if (activeSkillData[1] == SeotdaHwatuCombination.blank)
                activeSkillData[1] = skill;
            activeSkillCnt++;

            UpdateActiveSkillSlot();
        }
    }

    public void DeleteSkill(SeotdaHwatuCombination skill)
    {
        for(int i=0; i<2; i++)
        {
            if (activeSkillData[i] == skill)
            {
                activeSkillData[i] = SeotdaHwatuCombination.blank;
                activeSkillCnt--;
                break;
            }
        }
        UpdateActiveSkillSlot();
    }

    public void DeleteAllSkill()
    {
        for (int i = 0; i < 2; i++)
        {
            activeSkillData[i] = SeotdaHwatuCombination.blank;
            activeSkillCnt--;
        }
        UpdateActiveSkillSlot();
    }

    private void SkillSwap()
    {
        //Data Swap
        SeotdaHwatuCombination temp = activeSkillData[0];
        activeSkillData[0] = activeSkillData[1];
        activeSkillData[1] = temp;
        UpdateActiveSkillSlot();

        //CoolTime UI Swap
        float tempTimer = timer[0];
        timer[0] = timer[1];
        timer[1] = tempTimer;
        for (int i = 0; i < coolTimeImg.Length; i++)
        {
            if (activeSkillData[i] == SeotdaHwatuCombination.blank)
                continue;
            coolTimeImg[i].gameObject.SetActive(timer[i] > 0.0f);
            StartCoroutine(CoolTimeFunc(skillDBDictionary[activeSkillData[i]].coolTime, i, false));
        }
    }

    //Use Skill
    private void Skill(int i)
    {
        PlayerSkill skill = Player.instance.GetComponent<PlayerSkill>();
        if (activeSkillData[i] == SeotdaHwatuCombination.blank)
            return;

        SkillDB data = skillDBDictionary[activeSkillData[i]];
        skill.UseSkill(data);

        //CoolTime UI setting
        timer[i] = data.coolTime;
        coolTimeImg[i].gameObject.SetActive(true);
        StartCoroutine(CoolTimeFunc(data.coolTime, i, true));
    }

    public void UpdateActiveSkillSlot()
    {
        for (int i = 0; i < 2; i++)
            activeSkillSlot[i].ClearSlot();
        for (int i = 0; i < 2; i++)
            activeSkillSlot[i].UpdateSlot(activeSkillData[i]);
    }

    public void UpdatePassiveSkillSlot()
    {
        for(int i=0; i<passiveSkillSlot.Length; i++)
            passiveSkillSlot[i].ClearSlot();
        for (int i = 0; i < passiveSkillData.Count; i++)
            passiveSkillSlot[i].UpdateSlot(passiveSkillData[i]);
    }

    IEnumerator CoolTimeFunc(float coolTime, int i, bool flag)
    {
        if (flag)
            timer[i] = coolTime;
        while (timer[i] > 0.0f)
        {
            timer[i] -= Time.deltaTime;
            coolTimeImg[i].img.fillAmount = timer[i] / coolTime;
            coolTimeImg[i].text.text = Math.Round(timer[i], 1).ToString();
            yield return new WaitForFixedUpdate();
        }
        coolTimeImg[i].gameObject.SetActive(false);
    }

    public void RollingAdvantage()
    {
        for(int i=0; i<2; i++)
        {
            timer[i] -= 0.5f;
        }
    }

    public bool PassiveCheck(SeotdaHwatuCombination skillName)
    {
        for(int i=0; i< passiveSkillData.Count; i++)
        {
            if (passiveSkillData[i] == skillName)
                return true;
        }
        return false;
    }

    public void ClearSkill()
    {   // hwatu
        DeleteAllCardData();
        // active
        DeleteAllSkill();
        // passive
        // todo
    }
}
