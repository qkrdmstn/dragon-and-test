using System;
using System.IO;
using UnityEngine;
using TMPro;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public GameObject[] saveDatas;
    public int curSelectedIndex;

    public GameObject[] startBtns;

    string path;
    string filename = "playerData_";

    PlayerData[] data;

    private void Awake()
    {
        #region Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
        #endregion

        path = Application.persistentDataPath + "/";
        data = new PlayerData[3];

        if (File.Exists(path + filename + "0")) return;

        for(int i = 0; i < 3; i++)
        {
            File.Create(path + filename + i);
            Debug.Log("새로운 파일이 생성되었습니다");
        }
    }

    private void Start()
    {
        SetStartBtn();
    }

    public void SetSelectSlotIdx(int index) => curSelectedIndex = index;

    public void LoadData(int index)
    {
        string jsonData = File.ReadAllText(path + filename + index);

        data[index] = JsonUtility.FromJson<PlayerData>(jsonData);

        // 로드된 데이터를 플레이어 정보에 입력해주는 과정이 필요함 -> 소지한 총에 대한 정보를 Initialize할 과정도 필요
    }

    public void SaveData()
    {
        if (ScenesManager.instance.GetSceneEnum() != SceneInfo.Town_1) return;

        data[curSelectedIndex] = data[curSelectedIndex] == null ?
                                        new PlayerData(0) :
                                        new PlayerData(data[curSelectedIndex].totalPlayTime);

        string jsonData = JsonUtility.ToJson(data[curSelectedIndex]);

        File.WriteAllText(path + filename + curSelectedIndex, jsonData);
    }

    public void StartLoadData(int index)
    {
        SetSelectSlotIdx(index);
        if(data[index] == null)
        {
            Player.instance.RestoreHP();
            Player.instance.refShield = 0;
            Player.instance.refMoney = 0;

            ItemManager.instance.LoadSaveGun();
        }
        else if (data[index] != null)
        {
            Player.instance.refCurHp = data[index].playerHP;
            Player.instance.refMoney = data[index].money;
            Player.instance.refShield = data[index].curShieldCnt;

            if (data[index].isClearTutorial)
            {
                UIManager.instance.SceneUI["Jokbo"].GetComponent<JokboUIGroup>().isPossibleJokbo = true;
                Player.instance.isClearTutorial = true;
            }

            ItemManager.instance.LoadSaveGun(data[index].gunItems); // 보유중인 총 로드
            ItemManager.instance.curHoldingHwatuDatas = data[index].hwatus;
            ItemManager.instance.refHwatuCardCnt = data[index].hwatus.Count;
            foreach(var data in data[index].activeSkill)
            {
                SkillManager.instance.active[data.Key] = data.Value;
            }
            foreach (var data in data[index].passiveSkill)
            {
                SkillManager.instance.passive[data.Key] = data.Value;
            }
        }
        StartCoroutine(IsLoadedStartData());
    }

    IEnumerator IsLoadedStartData()
    {
        yield return new WaitUntil(()=>ScenesManager.instance.IsCompletedLoadData(0));

        UIManager.instance.StartFade((int)SceneInfo.Town_1);
    }

    public void ClearData(int index)
    {
        if(File.Exists(path + filename + index))
        {
            File.WriteAllText(path + filename + index, "");
            SetDataUI();
        }
    }

    public void SetStartBtn()
    {
        bool isNotNull = false;
        for(int i=0; i<3; i++)
        {
            LoadData(i);
            if (data[i] != null) isNotNull = true;
        }

        if(isNotNull)
        {
            startBtns[0].SetActive(false);
            startBtns[1].SetActive(true);
            SetDataUI();
        }
        else
        {
            startBtns[0].SetActive(true);
            startBtns[1].SetActive(false);
        }
    }

    enum saveElements
    {
        chapterInfo,
        saveTime,
        playTime
    }
    public void SetDataUI()
    {
        for(int i=0; i<3;i++)
        {
            LoadData(i);
            TextMeshProUGUI[] txt = saveDatas[i].GetComponentsInChildren<TextMeshProUGUI>(true);
            
            if (data[i] == null)
            {
                txt[(int)saveElements.chapterInfo].text = "새 게임";
                txt[(int)saveElements.saveTime].text = "빈 슬롯을 선택하세요";
                txt[(int)saveElements.playTime].text = "";
            }
            else
            {
                txt[(int)saveElements.chapterInfo].text = data[i].chapterName;
                txt[(int)saveElements.saveTime].text = "저장 일시 : "
                                                    + data[i].date.year + "년 "
                                                    + data[i].date.month + "월 "
                                                    + data[i].date.day + "일 "
                                                    + data[i].date.hour + "시 "
                                                    + data[i].date.min + "분";
                txt[(int)saveElements.playTime].text = "플레이 타임 " + ConvertPlaytimeTohour(i) + ConvertPlaytimeTomin(i);
            }
        }
    }

    string ConvertPlaytimeTohour(int index)
    {
        int hour = (int)data[index].totalPlayTime / 3600;
        return hour.ToString()+"시간 ";
    }

    string ConvertPlaytimeTomin(int index)
    {
        int min = (int)data[index].totalPlayTime / 60;
        min %= 60;
        return min.ToString()+"분";
    }
}
