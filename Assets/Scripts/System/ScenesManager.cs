using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneInfo
{
    Start,
    Town_1,
    Battle_1,
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

    public int GetSceneNum()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public void ChangeScene(SceneInfo _sceneInfo)
    {
        switch (_sceneInfo)
        {
            case SceneInfo.Start:
                break;

            case SceneInfo.Town_1:
                UIManager.instance.SceneUI[0].SetActive(false);
                UIManager.instance.SceneUI[1].SetActive(true);
                break;

            case SceneInfo.Battle_1:
                UIManager.instance.SceneUI[1].SetActive(false);
                UIManager.instance.SceneUI[2].SetActive(true);
                break;
        }
        SceneManager.LoadScene(((int)_sceneInfo));
    }
}
