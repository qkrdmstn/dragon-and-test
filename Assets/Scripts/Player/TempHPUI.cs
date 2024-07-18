using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempHPUI : MonoBehaviour
{
    public Text txt;

    private void Update()
    {
        txt.text = "HP: " + Player.instance.curHP + "\n Shield: " + Player.instance.shield;
    }
}
