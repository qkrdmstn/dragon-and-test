using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
//using static UnityEditor.Rendering.CameraUI;

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
    public int skillCnt;
    public SeotdaHwatuCombination[] skillData;
    public float[] timer;

    [Header("Skill UI info")]
    [SerializeField] private Transform skillSlotParent;
    public SkillSlotUI[] skillSlot;
    [SerializeField] private SkillCoolTimeImg[] coolTimeImg;

    //[Header("Skill State info")]
    //public bool isSaving;
    //public bool saveSuccess;

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

        ////배틀 씬에서만 유지
        //if (ScenesManager.instance == null || ScenesManager.instance.GetSceneNum() == (int)SceneInfo.Battle_1_A
        //    || ScenesManager.instance.GetSceneNum() == (int)SceneInfo.Battle_1_B
        //    || ScenesManager.instance.GetSceneNum() == (int)SceneInfo.Battle_1_C)
        //{
        DontDestroyOnLoad(this.gameObject); //씬이 넘어가도 오브젝트 유지
    }

    async void Start()
    {   //Initialize
        InitializeSkillData();
        await LoadSkillDB();
        InitializeUI(); 
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
        skillData = new SeotdaHwatuCombination[]{ SeotdaHwatuCombination.blank, SeotdaHwatuCombination.blank };
        skillCnt = 0;
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
    }

    private void InitializeUI()
    {
        skillSlotParent = UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[3].transform;
        skillSlot = skillSlotParent.GetComponentsInChildren<SkillSlotUI>();
        Transform cooltimeUIParent = skillSlotParent.GetChild(1);
        coolTimeImg = new SkillCoolTimeImg[2];
        for (int i = 0; i < 2; i++)
        {
            skillSlot[i].ClearSlot();
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

    public void AddSkill(SeotdaHwatuCombination skill)
    {
        if(skillData[0] == SeotdaHwatuCombination.blank)
            skillData[0] = skill;
        else if (skillData[1] == SeotdaHwatuCombination.blank)
            skillData[1] = skill;
        skillCnt++;

        UpdateSkillSlot();
    }

    public void DeleteSkill(SeotdaHwatuCombination skill)
    {
        for(int i=0; i<2; i++)
        {
            if (skillData[i] == skill)
            {
                skillData[i] = SeotdaHwatuCombination.blank;
                skillCnt--;
                break;
            }
        }
        UpdateSkillSlot();
    }

    private void SkillSwap()
    {
        //Data Swap
        SeotdaHwatuCombination temp = skillData[0];
        skillData[0] = skillData[1];
        skillData[1] = temp;
        UpdateSkillSlot();

        //CoolTime UI Swap
        float tempTimer = timer[0];
        timer[0] = timer[1];
        timer[1] = tempTimer;
        for (int i = 0; i < coolTimeImg.Length; i++)
        {
            if (skillData[i] == SeotdaHwatuCombination.blank)
                continue;
            coolTimeImg[i].gameObject.SetActive(timer[i] > 0.0f);
            StartCoroutine(CoolTimeFunc(skillDBDictionary[skillData[i]].coolTime, i, false));
        }
    }

    //Use Skill
    private void Skill(int i)
    {
        PlayerSkill skill = Player.instance.GetComponent<PlayerSkill>();
        if (skillData[i] == SeotdaHwatuCombination.blank)
            return;

        SkillDB data = skillDBDictionary[skillData[i]];
        skill.UseSkill(data);

        //CoolTime UI setting
        timer[i] = data.coolTime;
        coolTimeImg[i].gameObject.SetActive(true);
        StartCoroutine(CoolTimeFunc(data.coolTime, i, true));
    }

    public void UpdateSkillSlot()
    {
        for (int i = 0; i < 2; i++)
            skillSlot[i].ClearSlot();
        for (int i = 0; i < 2; i++)
            skillSlot[i].UpdateSlot(skillData[i]);
    }

    IEnumerator CoolTimeFunc(float coolTime, int i, bool flag)
    {
        if (flag)
            timer[i] = coolTime;
        while (timer[i] > 0.0f)
        {
            timer[i] -= Time.deltaTime;
            coolTimeImg[i].img.fillAmount = timer[i] / coolTime;
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

    public bool haveSkill(SeotdaHwatuCombination skillName)
    {
        if (SkillManager.instance.skillData[0] == skillName || SkillManager.instance.skillData[1] == skillName)
            return true;
        else
            return false;
    }
}
