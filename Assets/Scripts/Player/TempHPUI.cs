using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempHPUI : MonoBehaviour
{
    public Text txt;
    public Player player;
    public PlayerSkill playerSkill;

    private void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
        playerSkill = GameObject.FindObjectOfType<PlayerSkill>();
    }
    private void Update()
    {
        txt.text = "Draw: " + playerSkill.drawCnt + "\n" + "HP: " + player.HP + "\n" + "MP: " + player.curMP + " / " + player.maxMP;
    }
}
