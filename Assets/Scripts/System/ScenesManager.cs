using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneInfo
{
    Start,
    Tutorial,
    Town_1,
    Battle_1,
    Puzzle_1,
    Boss_1
};

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager instance = null;
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
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public int GetSceneNum()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public void ChangeScene(SceneInfo _sceneInfo)
    {
        if(_sceneInfo != SceneInfo.Tutorial)
            GunManager.instance.SaveGunData();

        SceneManager.LoadScene(((int)_sceneInfo));
    }

    // 체인을 걸어서 이 함수는 매 씬마다 호출된다.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneInfo _sceneInfo = (SceneInfo)scene.buildIndex;

        //InventoryTest Scene Setting
        if (scene.buildIndex == 9)
            _sceneInfo = SceneInfo.Town_1;

        switch (_sceneInfo)
        {
            case SceneInfo.Start:
                break;

            case SceneInfo.Tutorial:
                UIManager.instance.SceneUI["Start"].SetActive(false);
                UIManager.instance.SceneUI["Tutorial"].SetActive(true);
                UIManager.instance.SceneUI["Inventory"].SetActive(true);
                UIManager.instance.curUIGroup = UIManager.instance.SceneUI["Tutorial"].GetComponent<UIGroup>();

                UIManager.instance.InitUIReference();
                GunManager.instance.Initialize();
                break;

            case SceneInfo.Town_1:
                UIManager.instance.SceneUI["Tutorial"].SetActive(false);
                UIManager.instance.SceneUI["Town_1"].SetActive(true);
                UIManager.instance.SceneUI["Inventory"].SetActive(true);
                UIManager.instance.curUIGroup = UIManager.instance.SceneUI["Town_1"].GetComponent<UIGroup>();

                UIManager.instance.InitUIReference();
                GunManager.instance.Initialize();
                break;

            case SceneInfo.Battle_1:
                UIManager.instance.SceneUI["Town_1"].SetActive(false);
                UIManager.instance.SceneUI["Battle_1"].SetActive(true);
                UIManager.instance.SceneUI["Inventory"].SetActive(true);
                UIManager.instance.curUIGroup = UIManager.instance.SceneUI["Battle_1"].GetComponent<UIGroup>();

                UIManager.instance.InitUIReference();
                GunManager.instance.Initialize();
                break;
            case SceneInfo.Puzzle_1:
                UIManager.instance.SceneUI["Town_1"].SetActive(false);
                UIManager.instance.SceneUI["Battle_1"].SetActive(true);
                UIManager.instance.SceneUI["Inventory"].SetActive(true);
                UIManager.instance.curUIGroup = UIManager.instance.SceneUI["Battle_1"].GetComponent<UIGroup>();

                UIManager.instance.InitUIReference();
                GunManager.instance.Initialize();
                break;
        }
    }
}
