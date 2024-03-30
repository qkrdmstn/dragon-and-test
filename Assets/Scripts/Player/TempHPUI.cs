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
        Debug.Log(ScenesManager.instance.GetSceneNum());
        //Invoke("initia", 2.0f);
        initia();
    }

    void initia()
    {
        player = GameObject.FindObjectOfType<Player>();
        Debug.Log(ScenesManager.instance.GetSceneNum());
        Debug.Log(player);
    }
    private void Update()
    {
        Debug.Log(player);
        Debug.Log(ScenesManager.instance.GetSceneNum());
        txt.text = "HP: " + player.curHP + " / " + player.maxHP + "\n" + "MP: " + player.curMP + " / " + player.maxMP + "\n BlankBullet: " + player.blankBulletNum;
    }
}
