using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{ 
    public static UIManager instance = null;
    public Fade fade;
    public DialogueDB dialogueDB;

    //public GameObject[] SceneUI;
    public SerializableDictionary<string, GameObject> SceneUI;
    public UIGroup curUIGroup;
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

        fade = GetComponentInChildren<Fade>();
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

    public void InitUIReference()
    {
        player = FindObjectOfType<Player>();
    }
}
