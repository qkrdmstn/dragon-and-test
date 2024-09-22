using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MaterialHwatuSlotUI : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
    , IBeginDragHandler
    , IEndDragHandler
    , IDragHandler
{
    [Header("Transform Info")]
    public HwatuData hwatuData;
    public Vector3 originPos;
    public Quaternion originRot;
    public Vector3 originScale;
    public int originSiblingIndex;

    public Vector3 dragOffset;
    public bool isSelected = false;
    public bool isTweening = false;
    public bool isDrag = false;

    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected || isTweening || isDrag)
            return;

        originSiblingIndex = transform.GetSiblingIndex(); // 가장 위에 렌더링되는 카드가 자연스럽도록 Sibling의 순서를 바꿈
        transform.SetAsLastSibling();

        MoveTransform(originPos + new Vector3(0, 100, 0), originRot, originScale, 0.25f, false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected || isTweening || isDrag)
            return;

        MoveTransform(0.25f, false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isSelected || isTweening)
            return;

        isDrag = true;
        dragOffset = transform.position - Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isSelected || isTweening)
            return;

        isDrag = false;
        RectTransform rectTransform = this.GetComponent<RectTransform>();
        BlanketUI blanketUI = transform.GetComponentInParent<BlanketUI>();
        BlanketInteraction blanketInteraction = FindObjectOfType<BlanketInteraction>();

        bool isInBlanket = blanketUI.IsInBlanket(rectTransform);
        bool isInTrashCan = blanketUI.IsInTrashCan(rectTransform);

        if (isInBlanket) //모포에 완전히 겹쳤다면,
        {
            bool isCombinationPossible = blanketInteraction.AddSelectedHwatu(this); //조합 체크

            if (isCombinationPossible) // 가능한 조합
            {
                Vector3 pos = Input.mousePosition + dragOffset;
                Quaternion rot = Quaternion.identity;
                Vector3 scale = new Vector3(1.5f, 1.5f, 1);
                MoveTransform(pos, rot, scale, 0.1f, true);
                SoundManager.instance.SetEffectSound(SoundType.UI, UISfx.Snap);
                transform.SetAsLastSibling();
            }
            else
                blanketInteraction.CancelSelectedHwatu();
        }
        else if (isInTrashCan) //쓰레기통에 겹치면
            blanketInteraction.DeleteHwatu(this);
        else
            blanketInteraction.CancelSelectedHwatu();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isSelected || isTweening)
            return;

        if (!isDrag)
            isDrag = true;

        Vector3 pos = Input.mousePosition + dragOffset;
        Quaternion rot = Quaternion.identity;
        Vector3 scale = new Vector3(2, 2, 1);
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

    public void MoveTransform(float dotweenTime = 0, bool tweenSetting = true) //OriginPos로 이동
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
        transform.SetSiblingIndex(originSiblingIndex);
    }

    private void TweenStart()
    {
        isTweening = true;
    }

    private void TweenEnd()
    {
        isTweening = false;
    }

    private void OnValidate()
    {
        if (hwatuData != null)
        {
            gameObject.name = hwatuData.name;
            gameObject.GetComponent<Image>().sprite = hwatuData.sprite;
        }
    }
}
