using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HwatuUI : MonoBehaviour
{
    public TextMeshProUGUI txt;

    // Start is called before the first frame update
    void Awake()
    {
        txt = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SkillManager.instance == null) return;

        txt.text = SkillManager.instance.materialCardCnt + " / " + SkillManager.instance.materialCardMaxNum;
    }
}
