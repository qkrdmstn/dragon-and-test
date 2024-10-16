using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using static UnityEditor.Progress;

public class ActiveSkillSlotUI : MonoBehaviour
    , IBeginDragHandler
    , IEndDragHandler
    , IDragHandler
{
    [Header("Skill Info")]
    [SerializeField] private Image skillImage;
    public SeotdaHwatuCombination data;

    [Header("Transform Info")]
    public Vector3 originPos;
    public Quaternion originRot;
    public Vector3 originScale;

    public Vector3 dragOffset;
    public bool isTweening = false;

    private BlanketInteraction blanketInteraction;

    private void Awake()
    {

        skillImage = GetComponent<Image>();

        originPos = GetComponent<RectTransform>().position;
        originRot = Quaternion.identity;
        originScale = new Vector3(1, 1, 1);

        data = SeotdaHwatuCombination.blank;
    }

    private void Start()
    {
        blanketInteraction = FindObjectOfType<BlanketInteraction>();
    }

    public void UpdateSlot(SeotdaHwatuCombination _data)
    {

        if (_data != SeotdaHwatuCombination.blank)
        {
            data = _data;
            skillImage.color = Color.white;
            skillImage.sprite = SkillManager.instance.skillSpriteDictionary[_data];
        }
        else
        {
            data = SeotdaHwatuCombination.blank;
            skillImage.color = Color.clear;
            skillImage.sprite = null;
        }
    }

    public void ClearSlot()
    {
        skillImage.sprite = null;
        skillImage.color = Color.clear;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!blanketInteraction.isBlanketInteraction || data == SeotdaHwatuCombination.blank)
            return;

        dragOffset = transform.position - Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!blanketInteraction.isBlanketInteraction || data == SeotdaHwatuCombination.blank)
            return;

        RectTransform rectTransform = this.GetComponent<RectTransform>();
        BlanketUI blanketUI = FindAnyObjectByType<BlanketUI>();
        bool isInTrashCan = blanketUI.IsInTrashCan(rectTransform);
        
        if (isInTrashCan) //쓰레기통에 겹치면, 스킬 데이터 삭제
            SkillManager.instance.DeleteSkill(data);

        MoveTransform(0.25f, true); //원래 자리로 돌려보내기
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!blanketInteraction.isBlanketInteraction || data == SeotdaHwatuCombination.blank || isTweening)
            return;        
        if (isTweening)
            return;

        Vector3 pos = Input.mousePosition + dragOffset;
        Quaternion rot = Quaternion.identity;
        Vector3 scale = new Vector3(1.5f, 1.5f, 1);
        MoveTransform(pos, rot, scale);
    }

    public void MoveTransform(Vector3 pos, Quaternion rot, Vector3 scale, float dotweenTime = 0, bool tweenSetting = true)
    {
        if (tweenSetting)
        {
            transform.DOMove(pos, dotweenTime).OnStart(TweenStart);
            transform.DORotateQuaternion(rot, dotweenTime);
            transform.DOScale(scale, dotweenTime).OnComplete(TweenEnd);
        }
        else
        {
            transform.DOMove(pos, dotweenTime);
            transform.DORotateQuaternion(rot, dotweenTime);
            transform.DOScale(scale, dotweenTime);
        }
    }

    public void MoveTransform(float dotweenTime = 0, bool tweenSetting = true)
    {
        if (tweenSetting)
        {
            transform.DOMove(originPos, dotweenTime).OnStart(TweenStart);
            transform.DORotateQuaternion(originRot, dotweenTime);
            transform.DOScale(originScale, dotweenTime).OnComplete(TweenEnd);
        }
        else
        {
            transform.DOMove(originPos, dotweenTime);
            transform.DORotateQuaternion(originRot, dotweenTime);
            transform.DOScale(originScale, dotweenTime);
        }
    }

    private void TweenStart()
    {
        isTweening = true;
    }

    private void TweenEnd()
    {
        isTweening = false;
    }
}
