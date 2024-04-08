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
        if (instance == null)
        { //생성 전이면
            instance = this; //생성
        }
        else if (instance != this)
        { //이미 생성되어 있으면
            Destroy(this.gameObject); //새로만든거 삭제
        }
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
