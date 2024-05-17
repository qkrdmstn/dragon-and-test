using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class temp : MonoBehaviour
{
    public static temp instance;
    public Text txt;

    public int killScore = 0;

    private void Awake()
    {

    }

    private void Start()
    {
        killScore = 0;
    }

    private void Update()
    {
        txt.text = "Kill: " + killScore;
    }
}
