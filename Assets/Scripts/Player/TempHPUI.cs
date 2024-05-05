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
        player = GameObject.FindObjectOfType<Player>();
    }
    private void Update()
    {
        txt.text = "HP: " + player.curHP + " / " + player.maxHP;
    }
}
