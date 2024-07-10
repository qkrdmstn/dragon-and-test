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
    public HwatuData hwatuData;
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;

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
        MoveTransform(pos + new Vector3(0, 100, 0), rot, scale, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MoveTransform(0.5f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
    }

    public void MoveTransform(Vector3 pos, Quaternion rot, Vector3 scale, float dotweenTime = 0)
    {
        transform.DOMove(pos, dotweenTime);
        transform.DORotateQuaternion(rot, dotweenTime);
        transform.DOScale(scale, dotweenTime);
    }

    public void MoveTransform(float dotweenTime = 0)
    {
        transform.DOMove(pos, dotweenTime);
        transform.DORotateQuaternion(rot, dotweenTime);
        transform.DOScale(scale, dotweenTime);
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
