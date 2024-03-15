using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempHPUI : MonoBehaviour
{
    public Text txt;
    public Player player;

    private void Start()
    {
        player = GameObject.FindObjectOfType<Player>();

    }
    private void Update()
    {
        txt.text = "HP = " + player.HP;
    }
}
