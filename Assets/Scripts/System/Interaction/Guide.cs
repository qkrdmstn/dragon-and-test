using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guide : MonoBehaviour
{
    public Animator anim;
    public RectTransform startTalkBubble;
    public GameObject triggerAction;
    public bool isFirst = false;
    public bool isStart = false;
    public bool isInteraction = false;

    void Update()
    {
        if (Player.instance.isTownStart)
        {
            TurnOffTrigger();
            return;
        }
        else if(Player.instance.isInteraction && Input.GetKeyDown(KeyCode.F))
        {
            TurnOffAnimBool("isInteraction");
            Player.instance.isTownStart = true;
            return;
        }
        else if(!isFirst && UIManager.instance.isEndFade)
        {
            isFirst = true;
        }
        else if (isFirst)
        {
            if (!isStart)
            {
                isStart = true;
                TurnOnAnimBool("isStart");
            }   // 플레이어 머리 위 말풍선 위치 조정
            Vector3 uiPos = Player.instance.transform.position + Vector3.up * 1.25f;
            startTalkBubble.position = Camera.main.WorldToScreenPoint(uiPos);
        }
    }
    public void TurnOffTrigger()
    {
        triggerAction.SetActive(false);
    }

    public void TurnOnAnimBool(string animName)
    {
        anim.SetBool(animName, true);
    }

    public void TurnOffAnimBool(string animName)
    {
        anim.SetBool(animName, false);
    }
}
