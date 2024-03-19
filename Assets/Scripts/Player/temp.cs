using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class temp : MonoBehaviour
{
    public float x1, x2, x3, x4;
    public float y1, y2, y3, y4;

    // Start is called before the first frame update
    void Start()
    {


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            func1();
        }
    }


    private void func1()
    {
        int[] arr = new int[4];

        //for(int i=0; i<arr.Length; i++)
        //{
        //    Debug.Log(arr[i]);
        //}

        foreach(int a in arr)
        {
            Debug.Log(a);
        }
    }
}
