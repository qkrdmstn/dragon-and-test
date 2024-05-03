using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlanketHwatuButton : MonoBehaviour
{
    public Hwatu hwatu;
    public Image image;
    public TextMeshProUGUI text;
    public Button button;
    public HwatuData[] hwatuData;

    public int hwatuIdx;
    // Start is called before the first frame update
    private void Awake()
    {
        image = transform.GetChild(0).GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<Button>();
        hwatuData = Resources.LoadAll<HwatuData>("HwatuData");
    }

    public void SetButtonImage(int idx)
    {
        hwatuIdx = idx;
        hwatu = hwatuData[idx].hwatu;
        image.sprite = hwatuData[idx].sprite;
        text.text = hwatuData[idx].info;
    }
}
