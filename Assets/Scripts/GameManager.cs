using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public Player player;

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

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.C) && ScenesManager.instance.GetSceneNum() != (int)SceneInfo.Battle_1_A)
        //{
        //    ScenesManager.instance.ChangeScene((SceneInfo)(ScenesManager.instance.GetSceneNum() + 1));
        //}
    }

    public void GoToScene(int _sceneInfo)
    {
        UIManager.instance.fade.ManageFade(_sceneInfo);
        //ScenesManager.instance.ChangeScene((SceneInfo)_sceneInfo);
        SoundManager.instance.ManageSound(_sceneInfo);

        UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[0].SetActive(false);
        UIManager.instance.SceneUI["Battle_1"].SetActive(false);

        // player 관련 변수 Init() 필..요 ---- 아래는 임시로 해둔겁니당
        Time.timeScale = 1f;
        GameManager.instance.player.curHP = GameManager.instance.player.maxHP;
        
        switch (_sceneInfo)
        {
            case (int)SceneInfo.Start:
                UIManager.instance.SceneUI["Start"].SetActive(true);
                break;
            case (int)SceneInfo.Town_1:
                UIManager.instance.SceneUI["Town_1"].SetActive(true);
                break;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void InitReference()
    {
        player = FindObjectOfType<Player>();
    }
}
