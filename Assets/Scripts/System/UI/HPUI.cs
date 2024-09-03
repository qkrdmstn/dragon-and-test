using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPUI : MonoBehaviour
{
    public Sprite[] HPSprites;
    public Image HPImage;
    
    private void Update()
    {

        if (Player.instance.curHP == 0)
        {
            HPImage.sprite = null;
            HPImage.color = Color.clear;
        }
        else
        {
            HPImage.sprite = HPSprites[Player.instance.curHP];
            HPImage.color = Color.white;

        }
    }
}
