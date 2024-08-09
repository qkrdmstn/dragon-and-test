using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneInfo
{
    Start,
    Town_1,
    Tutorial,
    Battle_1_A,
    Battle_1_B,
    Battle_1_C,
    Puzzle_1,
    Boss_1
};

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager instance = null;
    public bool isLoading = false;

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
        if(_sceneInfo != (int)SceneInfo.Tutorial)
            GunManager.instance.SaveGunData();

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

        StartCoroutine(SoundManager.instance.FadeInSound(_sceneInfo));
        Player.instance.InitbySceneLoaded(_sceneInfo);
        
        switch (_sceneInfo)
        {
            case SceneInfo.Start:
                return;

            case SceneInfo.Town_1:      // 1
                UIManager.instance.SceneUI["Start"].SetActive(false);
                UIManager.instance.SceneUI["Tutorial"].SetActive(false);
                UIManager.instance.SceneUI["Battle_1"].SetActive(false);

                UIManager.instance.SceneUI["Town_1"].SetActive(true);
                UIManager.instance.SceneUI["Inventory"].SetActive(true);
                UIManager.instance.curUIGroup = UIManager.instance.SceneUI["Town_1"].GetComponent<UIGroup>();
                break;

            case SceneInfo.Tutorial:    // 2
                UIManager.instance.SceneUI["Battle_1"].SetActive(true);
                UIManager.instance.SceneUI["Tutorial"].SetActive(true);
                UIManager.instance.SceneUI["Inventory"].SetActive(true);
                UIManager.instance.curUIGroup = UIManager.instance.SceneUI["Tutorial"].GetComponent<UIGroup>();
                break;

            case SceneInfo.Puzzle_1:    // 3
                UIManager.instance.SceneUI["Town_1"].SetActive(false);
                UIManager.instance.SceneUI["Battle_1"].SetActive(true);
                UIManager.instance.SceneUI["Inventory"].SetActive(true);
                UIManager.instance.curUIGroup = UIManager.instance.SceneUI["Battle_1"].GetComponent<UIGroup>();
                break;

            case SceneInfo.Battle_1_A:
            case SceneInfo.Battle_1_B:
            case SceneInfo.Battle_1_C:
                UIManager.instance.SceneUI["Town_1"].SetActive(false);
                UIManager.instance.SceneUI["Battle_1"].SetActive(true);
                UIManager.instance.SceneUI["Inventory"].SetActive(true);
                UIManager.instance.curUIGroup = UIManager.instance.SceneUI["Battle_1"].GetComponent<UIGroup>();
                break;
            
            case SceneInfo.Boss_1:
                UIManager.instance.SceneUI["Town_1"].SetActive(false);
                UIManager.instance.SceneUI["Battle_1"].SetActive(true);
                UIManager.instance.SceneUI["Inventory"].SetActive(true);
                UIManager.instance.curUIGroup = UIManager.instance.SceneUI["Battle_1"].GetComponent<UIGroup>();
                break;
        }
        GunManager.instance.Initialize();
        isLoading = false;
    }
}
