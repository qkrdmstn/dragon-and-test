using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class temp : MonoBehaviour
{
    public static temp instance = null;
    public Text txt;


    public int killScore = 0;

    private void Awake()
    {
        if (instance == null)
        { //���� ���̸�
            instance = this; //����
        }
        else if (instance != this)
        { //�̹� �����Ǿ� ������
            Destroy(this.gameObject); //���θ���� ����
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
