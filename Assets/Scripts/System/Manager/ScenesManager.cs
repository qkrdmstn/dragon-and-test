using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneInfo
{
    Start,
    Town_1,
    Tutorial,
    Puzzle_1,
    Battle_1_A,
    Battle_1_B,
    Battle_1_C,
    Boss_1
};

[System.Serializable]
public struct UIState
{
    public UI myUI;
    public bool state;
}

[System.Serializable]
public class SceneInfos
{
    public SceneInfo myScene;
    public UIState[] curSceneUIState;
    public int loadDBcnt;   // 각 씬마다 로드될 DB가 다 불러와지면 ++되고 지정한 숫자가 되면 fadeOut됩니다
}

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager instance = null;
    public bool isLoading = false;
    public int isLoadedDB = 0;
    public List<SceneInfos> sceneInfos = new List<SceneInfos>();
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

    private void OnEnable()
    {
        // 씬 매니저의 sceneLoaded에 체인을 건다.
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnLoaded;
    }

    public int GetSceneNum()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public SceneInfo GetSceneEnum()
    {
        return (SceneInfo)SceneManager.GetActiveScene().buildIndex;
    }

    public void ChangeScene(int _sceneInfo)
    {
        SceneManager.LoadScene(_sceneInfo);
    }

    void OnSceneUnLoaded(Scene scene)
    {
        StartCoroutine(SoundManager.instance.FadeOutSound());
    }

    // 체인을 걸어서 이 함수는 매 씬마다 호출된다.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneInfo _sceneInfo = (SceneInfo)scene.buildIndex;

        //InventoryTest Scene Setting
        if (scene.buildIndex == 9)
            _sceneInfo = SceneInfo.Town_1;

        if (scene.buildIndex == 6 || scene.name == "BossTest")
            _sceneInfo = SceneInfo.Boss_1;

        StartCoroutine(SoundManager.instance.FadeInSound(_sceneInfo));
        Player.instance.InitbySceneLoaded(_sceneInfo);

        foreach(UIState ui in sceneInfos[(int)_sceneInfo].curSceneUIState)
        {
            ManageActiveUI(ui.myUI.ToString(), ui.state);
        }

        string curSceneName = sceneInfos[(int)_sceneInfo].myScene.ToString();
        if (curSceneName.Contains("Battle_1") || curSceneName.Contains("Puzzle_1") || curSceneName.Contains("Boss_1")) curSceneName = "Battle_1";
        UIManager.instance.curUIGroup = UIManager.instance.SceneUI[curSceneName].GetComponent<UIGroup>();

        isLoading = false;
    }

    void ManageActiveUI(string name, bool state)
    {
        UIManager.instance.SceneUI[name].SetActive(state);
    }

    public bool IsCompletedLoadData(int curScene)
    {
        if (isLoadedDB == sceneInfos[curScene].loadDBcnt)
        {
            isLoadedDB = 0;
            return true;
        }
        else return false;
    }
}
