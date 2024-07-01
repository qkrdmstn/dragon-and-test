using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempHPUI : MonoBehaviour
{
    public Text txt;
    private Player player;

    private void Start()
    {
        
    }
    private void Update()
    {
        txt.text = "HP: " + GameManager.instance.player.curHP + "\n Shield: " + GameManager.instance.player.shield;
    }
}
