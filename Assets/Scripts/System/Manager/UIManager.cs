using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum UI
{
    Dead, Dialogue, Inventory, GameExit
}

public enum PresenterType
{
    Player, Skill, Item
}

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;
    public GameObject fadeObj;
    public Fade fade;
    public bool isEndFade = false;
    public bool isFading = false;

    public List<PresenterBase> presenters;
    public SerializableDictionary<string, GameObject> SceneUI;
    public Stack<GameObject> curOpenUI;

    public bool isClose = false;
    GameObject gameExit;
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
        curOpenUI = new Stack<GameObject>();
        gameExit = SceneUI["GameExit"];
        exitDesc = gameExit.GetComponentsInChildren<TextMeshProUGUI>()[1];
    }

    private void Update()
    {
        if (isFading && isEndFade)
            SetFadeObjState(false);

        if (!Player.instance.isDead && !Player.instance.isTutorial && Input.GetKeyDown(KeyCode.Escape))
        {
            if (curOpenUI.Count == 0)
            {
                int curScene = ScenesManager.instance.GetSceneNum();
                if (isFading || curScene == 0 || curScene == 3) return;

                if (!gameExit.activeSelf)
                    SetActiveExitUI(true);
            }
            else if (gameExit.activeSelf)
                SetActiveExitUI(false);
        }
    }

    private void LateUpdate()
    {
        if (curOpenUI.Count > 0 && isClose)
        {
            PushPopUI();
            isClose = false;
        }
    }

    public void PushPopUI(GameObject ui = null)
    {
        if (ui == null)
        {
            if (curOpenUI.Count == 1)
            {   // last ui closed -> player movable
                Player.instance.ChangePlayerInteractionState(false);
            }
            GameObject tmp = curOpenUI.Pop();
            tmp.SetActive(false);
        }
        else
        {
            ui.SetActive(true);
            curOpenUI.Push(ui);
            if (curOpenUI.Count == 1)
                Player.instance.ChangePlayerInteractionState(true);
        }
    }

    public void ActivatePresentersUI(PresenterType presenterType, int idx, bool state)   // 어떤 presenter의 어떤 view?
    {
        presenters[(int)presenterType].objs[idx].SetActive(state);
    }

    public bool GetPresentersUIState(PresenterType presenterType, int idx)
    {
        return presenters[(int)presenterType].objs[idx].activeSelf;
    }

    public void SetActiveExitUI(bool visible)
    {
        if(visible)
            PushPopUI(gameExit);
        else
            isClose = true;

        if (ScenesManager.instance.GetSceneNum() != 1)
            exitDesc.text = "저장은 \"마을\" 에서만 가능합니다.\n이외의 장소에서 종료시, 데이터는 저장되지 않습니다.";
        else
            exitDesc.text = "데이터를 저장합니다.";

        GameManager.instance.SetTimeScale(visible ? 0f : 1f);
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

    public Texture2D ScaleTexture(Texture2D source, float _scaleFactor)
    {
        if (_scaleFactor == 1f)
        {
            return source;
        }
        else if (_scaleFactor == 0f)
        {
            return Texture2D.blackTexture;
        }

        int _newWidth = Mathf.RoundToInt(source.width * _scaleFactor);
        int _newHeight = Mathf.RoundToInt(source.height * _scaleFactor);



        Color[] _scaledTexPixels = new Color[_newWidth * _newHeight];

        for (int _yCord = 0; _yCord < _newHeight; _yCord++)
        {
            float _vCord = _yCord / (_newHeight * 1f);
            int _scanLineIndex = _yCord * _newWidth;

            for (int _xCord = 0; _xCord < _newWidth; _xCord++)
            {
                float _uCord = _xCord / (_newWidth * 1f);

                _scaledTexPixels[_scanLineIndex + _xCord] = source.GetPixelBilinear(_uCord, _vCord);
            }
        }

        // Create Scaled Texture
        Texture2D result = new Texture2D(_newWidth, _newHeight, source.format, false);
        result.SetPixels(_scaledTexPixels, 0);
        result.Apply();

        return result;
    }
}
