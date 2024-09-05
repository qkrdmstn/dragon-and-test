using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolTimeImg : MonoBehaviour
{
    public Image img;
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Awake()
    {
        img = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }


}
