using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Puzzle1Manager : MonoBehaviour
{
    public bool[] isClear;
    public GameObject portal;
    /*      
     * 필요한 기능
     * 2. 성공에 대한 보스 맵 이동 활성화
     * 3. 실패에 대한 플레이어 패널티 기능
     */

    private void Start()
    {
        isClear = new bool[2];
    }

    public void ClearCheck()
    {   // 특정 행동을 하면 ~
        if(isClear[(int)StoneTotem.TotemType.Main] & isClear[(int)StoneTotem.TotemType.Sub])
            Puzzle1Clear();
        else Fail();
    }

    void Puzzle1Clear()
    {
        portal.SetActive(true);
        Debug.Log("Clear!!");
    }

    void Fail()
    {
        Debug.Log("fail...");
    }
}
