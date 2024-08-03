using System.IO;
using System.Collections.Generic;
using UnityEngine;

//GoogleAPIs
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;
using System; // for CancellationToken Struct

[System.Serializable]
public enum SheetType
{
    Dialog,
    Tutorial,
    SpawnDB,
    SkillDB,
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [Header("스크립트에서 enum을 등록후, 시트ID를 추가합니다.")]
    public SerializableDictionary<SheetType, string> sheetIDs =  new SerializableDictionary<SheetType, string>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }


    #region Access Google Sheets
    private SheetsService _service = null;

    private string _data_token_folder;
    private string _data_client;                         // total path
    private const string _data_json = "client_secret";   // json file name for access
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

    public string SelectData(SheetType sheetType, string range)
    {   // 단일 데이터 반환
        if (!_is_credential)
            DoCredential();

        var select = _service.Spreadsheets.Values.Get(sheetIDs[sheetType], range);
        var response = select.Execute();
        
        return response.Values[0][0].ToString();
    }

    public IList<IList<object>> SelectDatas(SheetType sheetType, string range)
    {   // 원하는 범위의 데이터 반환
        if (!_is_credential)
            DoCredential();
        Debug.Log("type: " + sheetType + " range: " + range);
        var select = _service.Spreadsheets.Values.Get(sheetIDs[sheetType], range);
        var response = select.Execute();
        return response.Values;
    }

    public void InsertData(SheetType sheetType, string range, ref List<IList<object>> list_data)
    {
        if (!_is_credential)
            DoCredential();

        var valueRange = new ValueRange()
        {
            MajorDimension = "ROWS",
            Values = list_data // 추가할 데이터
        };

        var update = _service.Spreadsheets.Values.Update(valueRange, sheetIDs[sheetType], range);
        update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
        update.Execute();
    }

    public void ClearData(SheetType sheetType, string range)
    {   // 지정한 범위의 데이터를 삭제
        if (!_is_credential)
            DoCredential();

        ClearValuesRequest what = new ClearValuesRequest();
        var deleteAll = _service.Spreadsheets.Values.Clear(what, sheetIDs[sheetType], range);
        deleteAll.Execute();
    }
    #endregion

    public SpawnDB SplitContext(IList<object> data)
    {
        int blockNum = int.Parse(data[(int)ExcelStructure.blockNumber].ToString());
        int wave = int.Parse(data[(int)ExcelStructure.wave].ToString());
        string monsterType = data[(int)ExcelStructure.monsterType].ToString();
        int posX = int.Parse(data[(int)ExcelStructure.gridPosX].ToString());
        int posY = int.Parse(data[(int)ExcelStructure.gridPosY].ToString());
        Vector2Int spawnPosition = new Vector2Int(posX, posY);

        return new SpawnDB(blockNum, wave, monsterType, spawnPosition);
    }

    public SkillDB SplitContextSkill(IList<object> data)
    {
        SeotdaHwatuCombination synergyCode = (SeotdaHwatuCombination)Enum.Parse(typeof(SeotdaHwatuCombination),data[0].ToString()); 
        string synergyName = data[1].ToString(); 
        string info = data[2].ToString(); 
        int damage = int.Parse(data[3].ToString());
        float coolTime = float.Parse(data[4].ToString());
        float range = float.Parse(data[5].ToString());
        float force = float.Parse(data[6].ToString());
        float duration = float.Parse(data[7].ToString());
        float speed = float.Parse(data[8].ToString()); 
        float period = float.Parse(data[9].ToString());

        return new SkillDB(synergyCode, synergyName, info, damage, coolTime, range, force, duration, speed, period);
    }

    public DialogueDBEntity SplitContextDialog(IList<object> data)
    {
        int eventOrder = int.Parse(data[0].ToString());
        string eventName = data[1].ToString();
        string npcName = data[2].ToString();
        string dialogue = data[3].ToString();
        bool isSelect = bool.Parse(data[4].ToString());
        string selectType = "";
        if (isSelect)
        {
            selectType = data[5].ToString();
            DialogueDBEntity tmp = new DialogueDBEntity(eventOrder, eventName, npcName, dialogue, isSelect, selectType);
            return tmp;
        }
        else
        {
            DialogueDBEntity tmp = new DialogueDBEntity(eventOrder, eventName, npcName, dialogue, isSelect);
            return tmp;
        }
    }

    public TutorialDBEntity SplitContextTutorial(IList<object> data)
    {
        int eventOrder = int.Parse(data[0].ToString());
        int sequence = int.Parse(data[1].ToString());
        string dialogue = data[2].ToString();
        bool isInteraction = bool.Parse(data[3].ToString());

        return new TutorialDBEntity(eventOrder , sequence , dialogue, isInteraction);
    }
}
