using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Effect", menuName = "Data/ItemEffect/HealthRecovery")]
public class HealthRecovery : ItemEffect
{
    public int increaseHP = 1;

    public override void ExcuteEffect()
    {
        base.ExcuteEffect();
        //if (GameManager.instance.player.curHP == GameManager.instance.player.maxHP) return;
        GameManager.instance.player.curHP += 1;

        Debug.Log("Health Recovery Effect");
    }
}
