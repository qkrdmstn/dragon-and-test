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
    public HwatuData[] hwatuData; //전체 카드 데이터
    public List<HwatuData> materialHwatuDataList; //조합 카드 데이터
    public int materialCardCnt = 0;
    public int materialCardMaxNum = 10;

    [Header("DB")]
    [SerializeField] SkillBalanceTable skillTable;

    [Header("Skill Data")]
    public int skillCnt;
    //public HwatuData[] hwatuCardSlotData;
    //public SeotdaHwatuCombination curSynergy;
    //public SkillBalanceEntity[] skillData;
    //public float[] timer;

    //[Header("Skill UI info")]
    //[SerializeField] private Transform skillSlotParent;
    //private SkillSlotUI[] skillSlot;
    //[SerializeField] private SkillCoolTimeImg[] coolTimeImg;

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

        //배틀 씬에서만 유지
        if (ScenesManager.instance == null || ScenesManager.instance.GetSceneNum() == (int)SceneInfo.Battle_1_A
            || ScenesManager.instance.GetSceneNum() == (int)SceneInfo.Battle_1_B
            || ScenesManager.instance.GetSceneNum() == (int)SceneInfo.Battle_1_C)
        {
            DontDestroyOnLoad(this.gameObject); //씬이 넘어가도 오브젝트 유지
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Initialize
        InitializeSkillData();
    }

    #region Initialize Func
    private void InitializeSkillData()
    {
        hwatuData = Resources.LoadAll<HwatuData>("HwatuData");
        skillTable = Resources.Load<SkillBalanceTable>("SkillDB/SkillBalanceTable");

        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                if (skillTable.SkillTableEntity[i].cardName == hwatuData[j].hwatu.type.ToString())
                {
                    hwatuData[j].info = skillTable.SkillTableEntity[i].info;
                    break;
                }
            }
        }

        //timer = new float[2];

        //hwatuCardSlotData = new HwatuData[2];
        //skillData = new SkillBalanceEntity[2];
        //skillCnt = 0;
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
}
