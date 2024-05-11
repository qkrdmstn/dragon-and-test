using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Puzzle1Manager : MonoBehaviour
{
    public StoneTotem[] stoneTotems;

    [SerializeField] private GameObject StoneTotemParent;
    // Start is called before the first frame update
    void Start()
    {
        stoneTotems = StoneTotemParent.GetComponentsInChildren<StoneTotem>();
    }

    public void ClearCheck()
    {
        bool clear = true;
        for (int i = 0; i < 4; i++)
        {
            if (!stoneTotems[i].isClear)
            {
                clear = false;
                break;
            }
        }

        if (clear)
            Puzzle1Clear();
    }

    private void Puzzle1Clear()
    {
        Debug.Log("Clear!!");
    }
}
