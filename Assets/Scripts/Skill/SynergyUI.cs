using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SynergyUI : MonoBehaviour
{
    public Image[] images;
    public TextMeshProUGUI synergyNameTxt;
    public TextMeshProUGUI synergyInfoTxt;

    // Start is called before the first frame update
    void Awake()
    {
        images = new Image[2];
        images[0] = transform.GetChild(0).GetComponent<Image>();
        images[1] = transform.GetChild(1).GetComponent<Image>();

        synergyNameTxt = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        synergyInfoTxt = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
    }

    public void UpdateSynergyUI(SynergyEntity entity, HwatuData[] hwatuDatas)
    {
        images[0].sprite = hwatuDatas[0].sprite;
        images[1].sprite = hwatuDatas[1].sprite;

        Debug.Log(entity);
        synergyNameTxt.text = entity.synergyName;
        synergyInfoTxt.text = entity.info;
    }
}
