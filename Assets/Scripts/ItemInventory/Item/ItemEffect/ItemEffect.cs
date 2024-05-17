using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Effect", menuName = "Data/ItemEffect")]
public class ItemEffect : ScriptableObject
{
    public void ExcuteEffect()
    {
        Debug.Log("Excute Effect");
    }
}
