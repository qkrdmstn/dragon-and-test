using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldUI : MonoBehaviour
{
    public Sprite[] ShieldSprites;
    public Image ShieldImage;

    private void Update()
    {
        if(Player.instance.shield == 0)
        {
            ShieldImage.sprite = null;
            ShieldImage.color = Color.clear;
        }
        else
        {
            ShieldImage.sprite = ShieldSprites[Player.instance.shield];
            ShieldImage.color = Color.white;
        }
    }
}
