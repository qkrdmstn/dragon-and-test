using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Hwatu Data", menuName = "Data/Hwatu")]
public class HwatuData : ScriptableObject, IComparable
{
    public Hwatu hwatu;
    public Sprite sprite;
    public string info;

    public int CompareTo(object obj)
    {
        SeotdaHwatuName thisType = hwatu.type;
        SeotdaHwatuName compareType = (obj as HwatuData).hwatu.type;
        if (((int)thisType) < ((int)compareType))
            return -1;
        else
            return 1;
    }
}
