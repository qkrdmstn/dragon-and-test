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
        HPImage.sprite = HPSprites[Player.instance.curHP];
    }
}
