using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Effect", menuName = "Data/ItemEffect/MoneyAdd")]
public class AddMoney : ItemEffect
{
    public int money;

    public override void ExcuteEffect()
    {
        base.ExcuteEffect();
        Player.instance.money += money;

        Debug.Log("Money Adding Effect");
    }
}
