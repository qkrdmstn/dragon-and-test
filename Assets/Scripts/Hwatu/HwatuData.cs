using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Hwatu Data", menuName = "Data/Hwatu")]
public class HwatuData : ScriptableObject
{
    public Hwatu hwatu;
    public Sprite sprite;
    public string info;
}
