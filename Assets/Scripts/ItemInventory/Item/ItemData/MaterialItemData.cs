using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Material")]
public class MaterialItemData : ItemData
{
    public ItemEffect[] itemEffects;

    public void ItemEffect()
    {
        foreach(var item in itemEffects)
        {
            item.ExcuteEffect();
        }
    }
}
