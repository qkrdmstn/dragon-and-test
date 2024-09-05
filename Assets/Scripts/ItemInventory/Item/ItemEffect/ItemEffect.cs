using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Effect", menuName = "Data/ItemEffect")]
public class ItemEffect : ScriptableObject
{
    public virtual void ExcuteEffect()
    {
        Debug.Log("Excute Effect");
    }

    public virtual void ExcuteEffect(int amount)
    {
        Debug.Log("Excute Effect - amount "+ amount);
    }
}
