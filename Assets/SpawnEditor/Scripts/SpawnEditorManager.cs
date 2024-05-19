using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;

public enum SpawnEditor_State
{
    BlockSetting,
    WaveSetting
}

public class SpawnEditorManager : MonoBehaviour
{
    public static SpawnEditorManager instance;

    [Header("Save Load info")]
    [SerializeField] private string fileName;
    [SerializeField] private List<Dictionary<string, object>> spawnTable;
    

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
        curMonsterType = "BirdWarrior";

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
        List<string[]> data = new List<string[]>();

        string[] typeName;
        typeName = new string[5];
        typeName[0] = "blockNumber";
        typeName[1] = "wave";
        typeName[2] = "monsterType";
        typeName[3] = "gridPosX";
        typeName[4] = "gridPosY";
        data.Add(typeName);

        for (int i = 0; i < blocks.Length; i++) //각 blockinfo에 포함된 spawn data 취합
        {
            for (int j = 0; j < blocks[i].blockSpawnData.Count; j++)
            {
                string[] tempData = new string[5];
                tempData[0] = blocks[i].blockSpawnData[j].blockNum.ToString();
                tempData[1] = blocks[i].blockSpawnData[j].wave.ToString();
                tempData[2] = blocks[i].blockSpawnData[j].monsterType.ToString();
                tempData[3] = blocks[i].blockSpawnData[j].spawnGridPos.x.ToString();
                tempData[4] = blocks[i].blockSpawnData[j].spawnGridPos.y.ToString();

                data.Add(tempData);
            }
        }

        string[][] output = new string[data.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = data[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            sb.AppendLine(string.Join(delimiter, output[i]));
        }

        string filepath = SystemPath.GetPath();

        if (!Directory.Exists(filepath))
        {
            Directory.CreateDirectory(filepath + "SpawnDB/");

        }

        StreamWriter outStream = System.IO.File.CreateText(filepath + "SpawnDB/" + fileName + ".csv");
        outStream.Write(sb);
        outStream.Close();
    }

    public void LoadSpawnData()
    {
        //Spawn Data Clear
        for (int i = 0; i < blocks.Length; i++) //각 blockinfo에 포함된 spawn data 취합
        {
            blocks[i].blockSpawnData.Clear();
        }

        //CSV Load
        spawnTable = CSVReader.Read("SpawnDB/" + fileName);
        for (int i = 0; i < spawnTable.Count; i++)
        {
            int blockNum = int.Parse(spawnTable[i]["blockNumber"].ToString());
            int wave = int.Parse(spawnTable[i]["wave"].ToString());
            string monsterType = spawnTable[i]["monsterType"].ToString();
            int posX = int.Parse(spawnTable[i]["gridPosX"].ToString());
            int posY = int.Parse(spawnTable[i]["gridPosY"].ToString());
            Vector2Int spawnPosition = new Vector2Int(posX, posY);

            blocks[blockNum].AddSpawnData(wave, monsterType, spawnPosition);
        }
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
}
