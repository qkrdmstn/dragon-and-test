using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Unity.VisualScripting;

public class MaterialHwatuSlotUI : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
    , IPointerClickHandler
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

    // Start is called before the first frame update
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

        MoveTransform(originPos + new Vector3(0, 100, 0), originRot, originScale, 0.25f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected || isTweening || isDrag)
            return;

        transform.SetSiblingIndex(originSiblingIndex);
        MoveTransform(0.25f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if (isTweening)
        //    return;

        //BlanketInteraction blanketInteraction = FindObjectOfType<BlanketInteraction>();
        //if (!isSelected)
        //{
        //    int cellNum = blanketInteraction.AddSelectedHwatu(this);
        //    if(cellNum >= 0) //추가 성공
        //    {
        //        //칸 위치 이동
        //        BlanketUI blanketUI = transform.GetComponentInParent<BlanketUI>();
        //        Vector3 pos = blanketUI.materialHwatuPlace[cellNum].transform.position;
        //        Quaternion rot = Quaternion.identity;
        //        Vector3 scale = new Vector3(4, 4, 1);
        //        MoveTransform(pos, rot, scale, 0.5f);
        //    }
        //    else
        //    {
        //        Debug.Log("Hwatu Select Fail");
        //    }
        //}
        //else if(isSelected)
        //{
        //    blanketInteraction.DeleteSelectedHwatu(this);
        //}
        //Debug.Log("Click");
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
        bool isInBlanket = blanketUI.IsInBlanket(rectTransform);
        bool isInTrashCan = blanketUI.IsInTrashCan(rectTransform);

        if (isInBlanket) //모포에 완전히 겹쳤다면,
        {
            BlanketInteraction blanketInteraction = FindObjectOfType<BlanketInteraction>();
            bool isCombinationPossible = blanketInteraction.AddSelectedHwatu(this); //조합 체크

            if (isCombinationPossible) //가능한 조합
            {
                Vector3 pos = Input.mousePosition + dragOffset;
                Quaternion rot = Quaternion.identity;
                Vector3 scale = new Vector3(1.5f, 1.5f, 1);
                MoveTransform(pos, rot, scale, 0.1f, true);

                transform.SetAsLastSibling();
            }
            else
            {
                MoveTransform(0.25f, true); //원래 자리로 돌려보내기
                transform.SetSiblingIndex(originSiblingIndex);
            }
        }
        else if (isInTrashCan) //쓰레기통에 겹치면
        {
            BlanketInteraction blanketInteraction = FindObjectOfType<BlanketInteraction>();
            blanketInteraction.DeleteHwatu(this);
        }
        else
        {
            MoveTransform(0.25f, true); //원래 자리로 돌려보내기
            transform.SetSiblingIndex(originSiblingIndex);
        }
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

    public void MoveTransform(float dotweenTime = 0, bool tweenSetting = true)
    {
        if(tweenSetting)
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

    private void OnValidate()
    {
        if (hwatuData != null)
        {
            gameObject.name = hwatuData.name;
            gameObject.GetComponent<Image>().sprite = hwatuData.sprite;
        }
    }
}
