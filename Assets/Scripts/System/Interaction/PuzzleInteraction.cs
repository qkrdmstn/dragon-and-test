using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleInteraction : Interaction
{
    public bool[] isClear;
    GameObject portal;
    Animator anim;
    /*      
     * 필요한 기능
     * 2. 성공에 대한 보스 맵 이동 활성화
     * 3. 실패에 대한 플레이어 패널티 기능
     */

    private void Start()
    {
        portal = GameObject.FindGameObjectWithTag("Portal");
        isClear = new bool[2];
    }

    public override void LoadEvent(InteractionData data)
    {
        anim = data.GetComponent<Animator>();
        portal = data.transform.parent.parent.Find("GoToBoss").gameObject;  // ㄹㅇ ㅋㅋ

        StartCoroutine(ClearCheck());
    }
 

    IEnumerator ClearCheck()
    {
        anim.SetBool("isOn", true);
        yield return new WaitForSeconds(1); 

        if (isClear[(int)StoneTotem.TotemType.Main] & isClear[(int)StoneTotem.TotemType.Sub])
            Puzzle1Clear();
        else Fail();

        isDone = true;
    }

    void Puzzle1Clear()
    {
        portal.SetActive(true);
        Debug.Log("Clear!!");
    }

    void Fail()
    {
        anim.SetBool("isOn", false);
        FailAttack();
        Debug.Log("fail...");
    }

    void FailAttack()
    {
        Player.instance.OnDamamged(1);
        // 레이저라던가 ..시각적 연출...
    }

}
