using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum UI
{
    Start, Town_1, Battle_1, Dialogue, Inventory, GameExit
}

public class UIManager : MonoBehaviour
{ 
    public static UIManager instance = null;
    public GameObject fadeObj;
    public Fade fade;
    public bool isEndFade = false;
    public bool isFading = false;
    public bool isUIOn = false;

    public SerializableDictionary<string, GameObject> SceneUI;
    //public UIGroup curUIGroup;
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

        if(isUIOn)
        {   // jokbo esc 누르고 플레이어 상호작용 상태 `false`로 바뀐 뒤,
            // 해당 update문으로 들어오기 때문에 바로 gameExit도 활성화되는 문제 해결을 위한 변수 생성
            // TODO -----  인벤토리도 똑같아서 해당 변수활용을 통해 문제 해결 필요
            isUIOn = false;
        }
        else if (!Player.instance.isInteraction && !Player.instance.isDead && !Player.instance.isTutorial && Input.GetKeyDown(KeyCode.Escape))
        {
            int curScene = ScenesManager.instance.GetSceneNum();
            if (isFading || curScene == 0 || curScene == 3) return;

            if (SceneUI["GameExit"].activeSelf)
            {
                SetActiveExitUI(false);
            }
            else
            {
                SetActiveExitUI(true);
            }
        }
    }

    public void SetActiveExitUI(bool visible)
    {
        if (ScenesManager.instance.GetSceneNum() != 1)
            exitDesc.text = "저장은 \"마을\" 에서만 가능합니다.\n이외의 장소에서 종료시, 데이터는 저장되지 않습니다.";
        else
            exitDesc.text = "데이터를 저장합니다.";

        SceneUI["GameExit"].SetActive(visible);
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
