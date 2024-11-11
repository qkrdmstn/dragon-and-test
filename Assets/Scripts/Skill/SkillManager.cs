using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

enum SkillActionType
{
    Active, Passive
}

public struct ActiveSlotData
{
    public SeotdaHwatuCombination combination;
    public float curCoolTime;
    public bool coolTimeFlag;

    public ActiveSlotData(SeotdaHwatuCombination c, float time)
    {
        combination = c;
        curCoolTime = time;
        coolTimeFlag = false;
    }
    public ActiveSlotData(SeotdaHwatuCombination c, float time, bool flag)
    {
        combination = c;
        curCoolTime = time;
        coolTimeFlag = flag;
    }
}

public class Active
{
    Dictionary<ActiveSkillSlot, ActiveSlotData> skill; // 실제 데이터
    public Action<ActiveSkillSlot, bool> action;

    public Active()
    {   // 생성자
        skill = new Dictionary<ActiveSkillSlot, ActiveSlotData>()
        {   // 스킬슬롯, 보유 액티브 스킬
            { ActiveSkillSlot.Q, new ActiveSlotData(SeotdaHwatuCombination.blank, 0.0f) }, { ActiveSkillSlot.E, new ActiveSlotData(SeotdaHwatuCombination.blank, 0.0f)}
        };
    }

    public Dictionary<ActiveSkillSlot, ActiveSlotData> refSkill
    {   // read only
        get { return skill; }
    }
    public ActiveSlotData this[ActiveSkillSlot slot]
    {   // indexer
        get { return skill[slot]; }
        set
        {
            skill[slot] = value;
            action.Invoke(slot, skill[slot].coolTimeFlag);
        }
    }
    public bool this[ActiveSkillSlot slot, bool flag]
    {   // indexer - cooltime flag
        get { return skill[slot].coolTimeFlag; }
        set
        {
            ActiveSlotData data = skill[slot];
            data.coolTimeFlag = value;
            skill[slot] = data;

            action.Invoke(slot, skill[slot].coolTimeFlag);
        }
    }
    public float this[ActiveSkillSlot slot, float curNum]
    {   // indexer - cooltime
        get { return skill[slot].curCoolTime; }
        set
        {
            ActiveSlotData data = skill[slot];
            data.curCoolTime = value;
            skill[slot] = data;
        }
    }
}

public class Passive
{
    Dictionary<SeotdaHwatuCombination, int> skill;  // 보유 패시브 스킬, 중첩 횟수
    public Action<int> action;

    public Passive()
    {
        skill = new Dictionary<SeotdaHwatuCombination, int>();
    }
    public Dictionary<SeotdaHwatuCombination, int> refSkill
    {   // read-only
        get { return skill; }
    }
    public int this[SeotdaHwatuCombination combination]
    {   // indexer
        get { return skill[combination]; }
        set
        {
            skill[combination] = value;
            action.Invoke((int)combination);
        }
    }
}

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance = null;
    
    [Header("Hwatu Data")]
    public HwatuData[] hwatuData; //전체 카드 데이터
    public GameObject hwatuItemObj;
    public List<HwatuData> materialHwatuDataList; //조합 카드 데이터

    int materialCardMaxNum = 10;
    int materialCardCnt;
    public int refMaterialCardCnt
    {
        get { return materialCardCnt; }
        set
        {
            materialCardCnt = Mathf.Clamp(value, 0, materialCardMaxNum);
            hwatuAction.Invoke();
        }
    }
    public Action hwatuAction;

    [Header("DB")]
    SkillDB[] datas;
    public Dictionary<SeotdaHwatuCombination, SkillDB> skillDBDictionary = new Dictionary<SeotdaHwatuCombination, SkillDB>();
    public Dictionary<SeotdaHwatuCombination, Sprite> skillSpriteDictionary = new Dictionary<SeotdaHwatuCombination, Sprite>();

    [Header("Skill Data")]
    public Active active = new Active();
    public Passive passive = new Passive();
    SkillPresenter skillpresenter;

    void Awake()
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
        await LoadSkillDB();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !Player.instance.isInteraction) //Skill Slot Swap
            SkillSwap();

        if (Input.GetKeyDown(KeyCode.Q) && !Player.instance.isInteraction)
        {
            if (active[ActiveSkillSlot.Q].coolTimeFlag)
                Debug.Log("Q Skill is cooldown time");
            else
                Skill(ActiveSkillSlot.Q);
        }

        if (Input.GetKeyDown(KeyCode.E) && !Player.instance.isInteraction)
        {
            if (active[ActiveSkillSlot.E].coolTimeFlag)
                Debug.Log("E Skill is cooldown time");
            else
                Skill(ActiveSkillSlot.E);
        }
    }

    #region Initialize Func
    private void InitializeSkillData()
    {   //화투 데이터 로드
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
        refMaterialCardCnt = 0;

        skillpresenter = UIManager.instance.presenters[1] as SkillPresenter;
    }

    private async Task LoadSkillDB()
    {   //스킬 DataBase 로드
        datas = await DataManager.instance.GetValues<SkillDB>(SheetType.SkillDB, "A1:L33");
        for (int i = 0; i < datas.Length; i++)
        {
            skillDBDictionary.Add(datas[i].TransStringToEnum(), datas[i]);
        }
        ScenesManager.instance.isLoadedDB++;
    }
    #endregion

    public void AddMaterialCardData(HwatuData _data)
    {
        if(refMaterialCardCnt >= materialCardMaxNum)
        {
            Debug.Log("The card data is full");
            return;
        }    

        materialHwatuDataList.Add(_data);
        refMaterialCardCnt = materialHwatuDataList.Count;
        materialHwatuDataList.Sort();
    }

    public void DeleteMaterialCardData(HwatuData _data)
    {
        materialHwatuDataList.Remove(_data);
        refMaterialCardCnt = materialHwatuDataList.Count;
        materialHwatuDataList.Sort();
    }

    public void DeleteAllCardData()
    {
        materialHwatuDataList.Clear();
        refMaterialCardCnt = materialHwatuDataList.Count;
    }

    public void AddSkill(SeotdaHwatuCombination skill)
    {
        if(skill == SeotdaHwatuCombination.KK0)
        {
            int damage = GetSkillDB(SeotdaHwatuCombination.KK0).damage;
            Player.instance.OnDamamged(damage);
        }
        else if (IsPassive(skill)) //Add Passive Skill
        {
            if (passive.refSkill.ContainsKey(skill)) passive[skill]++;
            else passive[skill] = 1;
        }
        else //Add Active Skill
        {
            if (active[ActiveSkillSlot.Q].combination == SeotdaHwatuCombination.blank)
            {
                active[ActiveSkillSlot.Q] = new ActiveSlotData(skill, skillDBDictionary[skill].coolTime);
            }
            else if (active[ActiveSkillSlot.E].combination == SeotdaHwatuCombination.blank)
            {
                active[ActiveSkillSlot.E] = new ActiveSlotData(skill, skillDBDictionary[skill].coolTime);
            }
        }
    }

    public void DeleteSkill(SeotdaHwatuCombination skill)
    {
        foreach (var data in active.refSkill)
        {
            if(data.Value.combination == skill)
            {
                active[data.Key] = new ActiveSlotData(SeotdaHwatuCombination.blank, 0.0f);
                break;
            }
        }
    }

    public void DeleteSkill(ActiveSkillSlot type)
    {
        active[type] = new ActiveSlotData(SeotdaHwatuCombination.blank, 0.0f);
    }

    public void DeleteAllSkill()
    {
        foreach (var data in active.refSkill)
        {
            active[data.Key] = new ActiveSlotData(SeotdaHwatuCombination.blank, 0.0f);
        }
    }

    private void SkillSwap()
    {   //Data Swap
        skillpresenter.StopAllCoroutines();

        ActiveSlotData temp = active[ActiveSkillSlot.Q];
        active[ActiveSkillSlot.Q] = active[ActiveSkillSlot.E];
        active[ActiveSkillSlot.E] = temp;
    }

    //Use Skill
    private void Skill(ActiveSkillSlot type)
    {
        PlayerSkill skill = Player.instance.GetComponent<PlayerSkill>();
        if (active[type].combination == SeotdaHwatuCombination.blank)
            return;

        SkillDB data = GetSkillDB(active[type].combination);
        float coolTime = skill.UseSkill(data);

        if (coolTime > 0.0f)
        {
            if(!Player.instance.isSuperman || active[type].combination == SeotdaHwatuCombination.GTT38) //초사이언 스킬쿨 초기화, 38 광땡은 예외
            {   //CoolTime UI setting
                active[type, flag:true] = true;
            }
        }
    }

    public bool IsFullActive()
    {
        if(active[ActiveSkillSlot.Q].combination != SeotdaHwatuCombination.blank && active[ActiveSkillSlot.E].combination != SeotdaHwatuCombination.blank)
        {
            return true;
        }
        return false;
    }

    public void RollingAdvantage()
    {
        foreach (var data in active.refSkill)
        {
            active[data.Key, curNum:0.5f] -= 0.5f;
        }
    }

    public void ClearCoolTimer(ActiveSkillSlot slot, float originTime)
    {
        active[slot, flag:false] = false;
        active[slot, curNum:originTime] = originTime;
    }

    public void ClearCoolTimer()
    {   
        foreach(var data in active.refSkill)
        {
            active[data.Key, flag:false] = false;
        }
    }

    public bool PassiveCheck(SeotdaHwatuCombination skillName) //패시브 보유 여부 확인
    {
        return passive.refSkill.ContainsKey(skillName) ? true : false;
    }

    public bool IsPassive(SeotdaHwatuCombination skillName) //해당 스킬이 패시브 스킬인지 확인
    {
        int skillNum = (int)skillName;
        if (skillNum >= 13 && skillNum <= 21)
            return true;
        else 
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

    public SkillDB GetSkillDB(SeotdaHwatuCombination skillName)
    {
        SkillDB data = skillDBDictionary[skillName];
        return data;
    }

    public float GetSkillProb(SeotdaHwatuCombination skillName)
    {
        SkillDB skillData = GetSkillDB(skillName);

        if (IsPassive(skillName) && passive.refSkill.ContainsKey(skillName))
            return skillData.probability + (passive.refSkill[skillName] - 1) * skillData.growCoefficient;
        else
            return skillData.probability;
    }

    public string GetSkillInfo(SeotdaHwatuCombination skillName, bool flag) //true면 성장계수 반영
    {
        SkillDB skillData = GetSkillDB(skillName);
        float prob = skillData.probability;
        string skillInfo = skillData.info;
        if(flag)
            skillInfo = skillInfo.Replace("probability", "<color=red>" + Math.Round((GetSkillProb(skillName) * 100), 1).ToString() + "%</color>");
        else
            skillInfo = skillInfo.Replace("probability", "<color=red>" + Math.Round((skillData.probability * 100), 1).ToString() + "%</color>");

        if (skillName == SeotdaHwatuCombination.AHES74)
        {
            if (flag)
                skillInfo = skillInfo.Replace("probability", "<color=red>" + GetSkillProb(skillName).ToString() + "</color>");
            else
                skillInfo = skillInfo.Replace("probability", "<color=red>" + skillData.probability.ToString() + "</color>");
        }

        return skillInfo;
    }
}
