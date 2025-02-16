using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Effect", menuName = "Data/ItemEffect/AddMoney")]
public class AddMoney_ItemEffect : ItemEffect
{
    public override void ExcuteEffect(int amount)
    {
        base.ExcuteEffect(amount);
        SoundManager.instance.SetEffectSound(SoundType.UI, UISfx.GetCoin);
        playerPresenter.AddMoney(amount);
        Debug.Log("Add Money Effect");
    }
}
