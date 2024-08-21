using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public enum UI
{
    Start, Tutorial, Town_1, Battle_1, Dialogue, Inventory, GameExit
}

public class UIManager : MonoBehaviour
{ 
    public static UIManager instance = null;
    public GameObject fadeObj;
    public Fade fade;
    public bool isEndFade = false;
    public bool isFading = false;

    public SerializableDictionary<string, GameObject> SceneUI;
    public UIGroup curUIGroup;
    TextMeshProUGUI exitDesc;

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
    private void Start()
    {
        exitDesc = SceneUI["GameExit"].GetComponentsInChildren<TextMeshProUGUI>()[1];
    }

    private void Update()
    {
        if (isFading && isEndFade)
        {
            SetFadeObjState(false);
        }

        if (ScenesManager.instance.GetSceneNum() > 0 && Input.GetKeyDown(KeyCode.Escape) && !Player.instance.isInteraction && !Player.instance.isTutorial)
        {
            if (isFading) return;
            if (ScenesManager.instance.GetSceneNum() != 1)
                exitDesc.text = "저장은 \"마을\" 에서만 가능합니다.\n이외의 장소에서 종료시, 데이터는 저장되지 않습니다.";
            else
                exitDesc.text = "데이터를 저장합니다.";

            if (SceneUI["GameExit"].activeSelf)
            {
                SceneUI["GameExit"].SetActive(false);
                GameManager.instance.SetTimeScale(1f);
            }
            else
            {
                SceneUI["GameExit"].SetActive(true);
                GameManager.instance.SetTimeScale(0f);
            }
        }
    }

    public void SetFadeObjState(bool state)
    {
        if (state)
        {
            isFading = true;
            isEndFade = false;
        }
        else
        {
            isEndFade = true;
            isFading = false;
        }
        fadeObj.SetActive(state);
    }

    public void StartFade(int sceneNum)
    {
        SetFadeObjState(true);
        ScenesManager.instance.isLoadedDB = 0;
        fade.ManageFade(sceneNum);
    }

    public Texture2D TextureFromSprite(Sprite sprite)
    {   // sprite to texture2D
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
            return sprite.texture;
    }
}
