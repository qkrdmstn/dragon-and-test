using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/EffecItem")]
public class EffectItemData : ItemData
{
    public ItemEffect[] itemEffects;

    public void ItemEffect()
    {
        foreach (var item in itemEffects)
        {
            item.ExcuteEffect();
        }
    }

    public void ItemEffect(int amount)
    {
        foreach (var item in itemEffects)
        {
            item.ExcuteEffect(amount);
        }
    }
}
