using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public bool isStop = false;

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

    public void GoToScene(int _sceneInfo)
    {
        UIManager.instance.StartFade(_sceneInfo);
        Player.instance.ReloadPlayer();
    }

    public void SetTimeScale(float value)
    {
        if (value > 0) isStop = false;
        else isStop = true;

        Time.timeScale = value;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
