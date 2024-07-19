using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Effect", menuName = "Data/ItemEffect/Shield")]
public class CreateShield_ItemEffect : ItemEffect
{
    public int createShield = 2;

    public override void ExcuteEffect()
    {
        base.ExcuteEffect();
        Player.instance.shield = createShield;

        Debug.Log("Create Shield Effect");
    }
}
