using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MaterialHwatuSlotUI : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
    , IPointerClickHandler
{
    [Header("Transform Info")]
    public HwatuData hwatuData;
    public Vector3 originPos;
    public Quaternion originRot;
    public Vector3 originScale;

    public bool isSelected = false;
    public bool isTweening = false;

    private Image image;

    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isTweening && !isSelected)
            MoveTransform(originPos + new Vector3(0, 100, 0), originRot, originScale, 0.25f, false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isTweening && !isSelected)
            MoveTransform(0.25f, false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isTweening)
            return;

        BlanketInteraction blanketInteraction = FindObjectOfType<BlanketInteraction>();
        if (!isSelected)
        {
            int cellNum = blanketInteraction.AddSelectedHwatu(this);
            if(cellNum >= 0) //추가 성공
            {
                //칸 위치 이동
                BlanketUI blanketUI = transform.GetComponentInParent<BlanketUI>();
                Vector3 pos = blanketUI.materialHwatuPlace[cellNum].transform.position;
                Quaternion rot = Quaternion.identity;
                Vector3 scale = new Vector3(4, 4, 1);
                MoveTransform(pos, rot, scale, 0.5f);
            }
            else
            {
                Debug.Log("Hwatu Select Fail");
            }
        }
        else if(isSelected)
        {
            blanketInteraction.DeleteSelectedHwatu(this);
        }
        Debug.Log("Click");
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
