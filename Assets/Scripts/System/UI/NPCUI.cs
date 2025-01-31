using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCUI : MonoBehaviour
{
    public TextMeshProUGUI txt;
    public Image talkBox;
    [TextArea] public string script;

    void Awake()
    {
        txt = GetComponentInChildren<TextMeshProUGUI>();
        talkBox = GetComponentInChildren<Image>();
    }

    void Start()
    {
        txt.text = script;
    }

    void Update()
    {
        talkBox.transform.position = Camera.main.WorldToScreenPoint(transform.position);
    }
}
