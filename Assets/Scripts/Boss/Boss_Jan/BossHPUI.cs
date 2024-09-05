using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHPUI : MonoBehaviour
{
    public Image img;
    // Start is called before the first frame update
    void Awake()
    {
        img = GetComponent<Image>();
    }

    public void UpdateHPUI(int curHP, int maxHP)
    {
        img.fillAmount = (float)curHP / (float)maxHP;
    }

}
