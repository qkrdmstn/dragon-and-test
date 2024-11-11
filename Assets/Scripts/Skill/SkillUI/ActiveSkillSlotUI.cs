using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActiveSkillSlotUI : MonoBehaviour
    , IBeginDragHandler
    , IEndDragHandler
    , IDragHandler
{
    [Header("Skill Info")]
    public SeotdaHwatuCombination data;
    public ActiveSkillSlot type;

    [Header("Transform Info")]
    public Vector3 originPos;
    public Quaternion originRot;
    public Vector3 originScale;

    public Vector3 dragOffset;
    public bool isTweening = false;

    private BlanketInteraction blanketInteraction;

    private void Awake()
    {
        originPos = GetComponent<RectTransform>().position;
        originRot = Quaternion.identity;
        originScale = new Vector3(1, 1, 1);

        data = SeotdaHwatuCombination.blank;
    }

    private void Start()
    {
        blanketInteraction = FindObjectOfType<BlanketInteraction>();
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
            SkillManager.instance.DeleteSkill(type);

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
