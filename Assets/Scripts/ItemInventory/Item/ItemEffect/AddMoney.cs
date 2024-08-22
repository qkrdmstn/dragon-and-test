using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Effect", menuName = "Data/ItemEffect/MoneyAdd")]
public class AddMoney : ItemEffect
{
    public override void ExcuteEffect(int amount)
    {
        base.ExcuteEffect(amount);
        Player.instance.money += amount;

        Debug.Log("Money Adding Effect");
    }
}
