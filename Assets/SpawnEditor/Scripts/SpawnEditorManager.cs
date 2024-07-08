using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

//GoogleAPIs
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading; // for CancellationToken Struct

public enum SpawnEditor_State
{
    BlockSetting,
    WaveSetting
}

public class SpawnEditorManager : MonoBehaviour
{
    public static SpawnEditorManager instance;  

    [Header("Monster Visualize info")]
    public GameObject[] monsterObj;

    [Header("Reference")]
    private SpawnEditor_Camera cam;
    private SpawnEditor_BlockInfo[] blocks;
    public SpawnEditor_BlockInfo totalBlock;

    [Header("State info")]
    public SpawnEditor_State curState;
 

    [Header("WaveSetting info")]
    public SpawnEditor_BlockInfo selectedBlock;
    public int curWave;
    public string curMonsterType;

    [Header("UI info")]
    public SpawnEditor_BlockSettingUI blockSettingUI;
    public SpawnEditor_WaveSettingUI waveSettingUI;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        cam = FindObjectOfType<SpawnEditor_Camera>();
        blockSettingUI = FindObjectOfType<SpawnEditor_BlockSettingUI>();
        waveSettingUI = FindObjectOfType<SpawnEditor_WaveSettingUI>();
        InitializeBlockInfo();


        SetState(SpawnEditor_State.BlockSetting);
        InitializeWaveSettingInfo();
    }

    #region Initialize
    private void InitializeWaveSettingInfo()
    {
        curWave = 0;
        curMonsterType = "BirdWarrior1";

        waveSettingUI.UpdateWaveInfoUI();
    }

    private void InitializeBlockInfo()
    {
        blocks = FindObjectsOfType<SpawnEditor_BlockInfo>();
        Array.Sort(blocks);

        for (int i = 0; i < blocks.Length; i++)
            blocks[i].InitializeBlockInfo(i);

        //total block
        float minX = blocks[0].min.x;
        float maxX = blocks[0].max.x;
        float minY = blocks[0].min.y;
        float maxY = blocks[0].max.y;
        for (int i = 1; i < blocks.Length; i++)
        {
            minX = Mathf.Min(minX, blocks[i].min.x);
            maxX = Mathf.Max(maxX, blocks[i].max.x);
            minY = Mathf.Min(minY, blocks[i].min.y);
            maxY = Mathf.Max(maxY, blocks[i].max.y);
        }
        Vector2 min = new Vector2(minX, minY);
        Vector2 max = new Vector2(maxX, maxY);

        totalBlock = new SpawnEditor_BlockInfo();
        totalBlock.InitializeBlockInfo(min, max);
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (curState == SpawnEditor_State.BlockSetting)
            BlockSettingInput();
        else if(curState == SpawnEditor_State.WaveSetting)
            WaveSettingInput();
    }

    private void BlockSettingInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                //Initial Direction Setting
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                for (int i = 0; i < blocks.Length; i++)
                {
                    if (blocks[i].IsInBlock(mousePos))
                    {
                        selectedBlock = blocks[i];
                        SetState(SpawnEditor_State.WaveSetting);
                        InitializeWaveSettingInfo();
                        selectedBlock.VisualizeSpawnData();
                        break;
                    }
                }
            }
        }
    }

    private void WaveSettingInput()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (selectedBlock.IsInBlock(mousePos))
                {
                    Vector2Int gridPos = selectedBlock.WorldToGridPosition(mousePos);
                    selectedBlock.AddSpawnData(curWave, curMonsterType, gridPos);
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (selectedBlock.IsInBlock(mousePos))
                {
                    Vector2Int gridPos = selectedBlock.WorldToGridPosition(mousePos);
                    selectedBlock.DeleteSpawnData(curWave, gridPos);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && curState == SpawnEditor_State.WaveSetting)
        {
            InitializeWaveSettingInfo();
            selectedBlock.VisualizeSpawnData();
            selectedBlock = null;
            SetState(SpawnEditor_State.BlockSetting);
        }
    }

    private void UISetting()
    {
        bool flag = false;
        if (curState == SpawnEditor_State.BlockSetting)
            flag = false;
        else if (curState == SpawnEditor_State.WaveSetting)
            flag = true;

        blockSettingUI.gameObject.SetActive(!flag);
        waveSettingUI.gameObject.SetActive(flag);
    }

    private void SetState(SpawnEditor_State state)
    {
        curState = state;
        cam.CameraSetting();
        UISetting();
    }

    #region Current Info Setting

    public void WaveIncrease()
    {
        if (selectedBlock.IsContinuousWave(curWave))
            curWave++;
        else
            Debug.LogWarning("The wave cannot be increased because there is an empty wave.");
        selectedBlock.VisualizeSpawnData();
    }

    public void WaveDecrease()
    {
        if (curWave >= 1)
            curWave--;
        else
            Debug.LogWarning("Current wave cannot be negative.");
        selectedBlock.VisualizeSpawnData();
    }

    public void SetCurMonsterType(string type)
    {
        this.curMonsterType = type;
    }
    #endregion

    #region Save & Load & Reset
    public void SaveSpawnData()
    {
        DataManager.instance.ClearData(SheetType.SpawnDB, "sheetA!A:E");

        List<IList<object>> paramLists = new List<IList<object>>();
        paramLists.Add( new List<object>()
        {
            "blockNumber", "wave", "monsterType", "gridPosX", "gridPosY"
        });

        for (int i = 0; i < blocks.Length; i++) //각 blockinfo에 포함된 spawn data 취합
        {
            for (int j = 0; j < blocks[i].blockSpawnData.Count; j++)
            {
                List<object> tempData = new List<object>()
                {
                    blocks[i].blockSpawnData[j].blockNum.ToString()
                    ,blocks[i].blockSpawnData[j].wave.ToString()
                    ,blocks[i].blockSpawnData[j].monsterType.ToString()
                    ,blocks[i].blockSpawnData[j].spawnPosition.x.ToString()
                    ,blocks[i].blockSpawnData[j].spawnPosition.y.ToString()
                };
                paramLists.Add(tempData);
            }
        }

        DataManager.instance.InsertData(SheetType.SpawnDB, "SheetA", ref paramLists);
    }

    public void LoadSpawnData()
    {
        //Spawn Data Clear in Unity
        for (int i = 0; i < blocks.Length; i++) //각 blockinfo에 포함된 spawn data 취합
        {
            blocks[i].blockSpawnData.Clear();
        }

        //Google Sheet Load
        string lastIdx = DataManager.instance.SelectData(SheetType.SpawnDB, "G2");
        IList<IList<object>> result = DataManager.instance.SelectDatas(SheetType.SpawnDB, "sheetA!A1:E" + lastIdx);
        SaveBlockSpawnData(result);
    }

    public void ResetSpawnData()
    {
        //Spawn Data Clear
        for (int i = 0; i < blocks.Length; i++) //각 blockinfo에 포함된 spawn data 취합
        {
            blocks[i].blockSpawnData.Clear();
            blocks[i].VisualizeSpawnData();
        }
    }
    #endregion

    private void SaveBlockSpawnData(IList<IList<object>> result)
    {
        for(int i = 1; i < result.Count; i++)
        {
            SpawnDB tmp = DataManager.instance.SplitContext(result[i]);

            blocks[tmp.blockNum].AddSpawnData(tmp.wave, tmp.monsterType, tmp.spawnPosition);
        }
    }
}


