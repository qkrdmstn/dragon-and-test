using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DialogueInteraction : PlayerInteraction
{
    protected override void LoadEvent()
    {
        Debug.Log(interation.eventName);
        //string _eventName = base.;
        // TODO
        // - NPC에게 뜬 느낌표 표시삭제 및 카메라 줌인&아웃같은 기능 필요
        // - 파일 파싱
        // - 대화 진행
        SetInteractionFlag(false);  // 상호작용 종료
    }
}