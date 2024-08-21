using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Puzzle1Manager : MonoBehaviour
{
    [SerializeField] private GameObject StoneTotemParent;
    public bool[] checkTotemClear;

    private void Awake()
    {
        checkTotemClear = new bool[4];
    }

    public void ClearCheck()
    {
        if(checkTotemClear[0] & checkTotemClear[1] & checkTotemClear[2] & checkTotemClear[3])
            Puzzle1Clear();
    }

    private void Puzzle1Clear()
    {
        Debug.Log("Clear!!");
    }
}
