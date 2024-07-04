using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        ClearData();

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
                    ,blocks[i].blockSpawnData[j].spawnGridPos.x.ToString()
                    ,blocks[i].blockSpawnData[j].spawnGridPos.y.ToString()
                };
                paramLists.Add(tempData);
            }
        }

        InsertData("SheetA", ref paramLists);
    }

    public void LoadSpawnData()
    {
        //Spawn Data Clear in Unity
        for (int i = 0; i < blocks.Length; i++) //각 blockinfo에 포함된 spawn data 취합
        {
            blocks[i].blockSpawnData.Clear();
        }

        //Google Sheet Load
        FindLastIndex();
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


    # region Access Google Sheets
    private SheetsService _service = null;

    private string _data_token_folder;
    private string _data_client;    // total path
    private const string _data_json = "client_secret";    // json file name for access
    private bool _is_credential;

    private void DoCredential()
    {
        _data_token_folder = "Assets/Resources/SpawnDB/";
        _data_client = _data_token_folder + _data_json + ".json";
 
        // 데이터의 가공 권한 범위 지정
        string[] arr_scope = { SheetsService.Scope.Spreadsheets };

        UserCredential credential;

        // Client 토큰 생성
        using (var stream = new FileStream(_data_client, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                arr_scope,
                "user",
                CancellationToken.None,
                new FileDataStore(_data_token_folder, true)).Result;
        }

        // API 서비스 생성
        _service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "DragonAndTest"
        });

        _is_credential = true;
    }

    const string _sheet_id = "1Oe1ybwebAoIVemBKFcvIttoWiu1WG074pUAP6cq7zP4";
    private void FindLastIndex()
    {
        if (!_is_credential)
            DoCredential();

        var select = _service.Spreadsheets.Values.Get(_sheet_id, "G2");
        var response = select.Execute();
        IList<IList<object>> result = response.Values;
        string lastIdx = result[0][0].ToString();

        SelectData(lastIdx);
    }

    private void SelectData(string lastIdx)
    {
        if (!_is_credential)
            DoCredential();

        string select_range = "sheetA!A1:E" + lastIdx;
        var select = _service.Spreadsheets.Values.Get(_sheet_id, select_range);
        var response = select.Execute();
        IList<IList<object>> result = response.Values;

        for(int i = 1; i < result.Count; i++)
        {
            int blockNum = int.Parse(result[i][(int)ExcelStructure.blockNumber].ToString());
            int wave = int.Parse(result[i][(int)ExcelStructure.wave].ToString());
            string monsterType = result[i][(int)ExcelStructure.monsterType].ToString();
            int posX = int.Parse(result[i][(int)ExcelStructure.gridPosX].ToString());
            int posY = int.Parse(result[i][(int)ExcelStructure.gridPosY].ToString());
            Vector2Int spawnPosition = new Vector2Int(posX, posY);

            blocks[blockNum].AddSpawnData(wave, monsterType, spawnPosition);
        }
    }

    private void InsertData(string str_sheet_range, ref List<IList<object>> list_data)
    {   
        if (!_is_credential)
            DoCredential();
        
        var valueRange = new ValueRange()
        {
            MajorDimension = "ROWS",
            Values = list_data // 추가할 데이터
        };

        var update = _service.Spreadsheets.Values.Update(valueRange, _sheet_id, str_sheet_range);
        update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
        update.Execute();
    }

    private void ClearData()
    {   // 지정한 범위의 데이터를 삭제
        if (!_is_credential)
            DoCredential();

        ClearValuesRequest what = new ClearValuesRequest();
        var deleteAll = _service.Spreadsheets.Values.Clear(what, _sheet_id, "sheetA!A:E");
        deleteAll.Execute();
    }
    #endregion
}


