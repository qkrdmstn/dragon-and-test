using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownInteraction : Interaction
{
    public Animator animUI;
    public Animator animArrow;
    public RectTransform startTalkBubble;
    public GameObject triggerAction;

    PlayerInteraction playerInteraction;

    bool isFirst = false;
    bool isStart = false;
    bool isInteraction = false;
    bool isMoveAnim = false;

    void Start()
    {
        playerInteraction = Player.instance.GetComponentInChildren<PlayerInteraction>();
    }

    void Update()
    {
        if (Player.instance.isTownStart)
        {
            return;
        }
        else if (isInteraction && !Player.instance.isInteraction)
        {
            TurnOnArrowAnim("isNeighborToTutorial");
            isInteraction = false;
        }
        else if (Player.instance.isBounded && Input.GetKeyDown(KeyCode.F))
        {
            if (playerInteraction.interaction.eventName.Equals("학관"))
            {
                animArrow.gameObject.SetActive(false);
                Player.instance.isTownStart = true;
            }
            else if(playerInteraction.interaction.eventName.Equals("주인공 집 근처"))
            {
                isInteraction = true;
                TurnOffUIAnim("isInteraction");
                TurnOffArrowAnim("isHouseToNeighbor");
                TurnOffTrigger();
            }
        }
        else if (!isFirst && UIManager.instance.isEndFade)
        {   // 마을 시작
            isFirst = true;
        }
        else if (isFirst)
        {
            if (!isStart)
            {
                isStart = true;
                TurnOnUIAnim("isStart");
            }   // 플레이어 머리 위 말풍선 위치 조정
            if (!isMoveAnim && FollowingAnimState())
            {
                isMoveAnim = true;
                TurnOnArrowAnim("isHouseToNeighbor");
            }
            Vector3 uiPos = Player.instance.transform.position + Vector3.up * 1.25f;
            startTalkBubble.position = Camera.main.WorldToScreenPoint(uiPos);
        }
    }

    public bool FollowingAnimState()
    {
        if(animUI.GetCurrentAnimatorStateInfo(0).IsName("Move"))
            return true;
        else return false;
    }

    public void TurnOffTrigger()
    {
        triggerAction.SetActive(false);
    }

    public void TurnOnUIAnim(string animName)
    {
        if (Player.instance.isTownStart)
            return;
        else animUI.SetBool(animName, true);
    }

    public void TurnOffUIAnim(string animName)
    {
        animUI.SetBool(animName, false);
    }

    public void TurnOnArrowAnim(string animName)
    {
        if (Player.instance.isTownStart)
            return;
        else animArrow.SetBool(animName, true);
    }

    public void TurnOffArrowAnim(string animName)
    {
        animArrow.SetBool(animName, false);
    }
}
