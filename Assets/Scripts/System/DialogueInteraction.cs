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
        // - NPC���� �� ����ǥ ǥ�û��� �� ī�޶� ����&�ƿ����� ��� �ʿ�
        // - ���� �Ľ�
        // - ��ȭ ����
        SetInteractionFlag(false);  // ��ȣ�ۿ� ����
    }
}