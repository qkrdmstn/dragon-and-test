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
        ShieldImage.sprite = ShieldSprites[Player.instance.shield];
    }
}
