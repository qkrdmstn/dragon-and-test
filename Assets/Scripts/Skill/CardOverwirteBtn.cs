using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardOverwirteBtn : MonoBehaviour
{
    public Hwatu hwatu;
    public Image image;
    public TextMeshProUGUI text;
    public Button button;

    // Start is called before the first frame update
    void Awake()
    {
        image = transform.GetChild(0).GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<Button>();
    }

    public void UpdateBtn(HwatuData data)
    {
        hwatu = data.hwatu;
        image.sprite = data.sprite;
        //text.text = data.info;
    }

}
