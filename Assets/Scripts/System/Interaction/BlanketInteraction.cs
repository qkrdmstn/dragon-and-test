using System.Collections;
using System.Collections.Generic;
//using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
//using Unity.VisualScripting;
//using UnityEditor.Hardware;
//using UnityEditor.Experimental.GraphView;

public class BlanketInteraction : Interaction
{
    private Player player;

    [Header("UI")]
    [SerializeField] private GameObject[] materialHwatuUIObject;
    [SerializeField] private GameObject blanketUI;

    [Header("UI")]
    [SerializeField] private RectTransform materialHwatuParent;
    [SerializeField] private Vector2 minPos;
    [SerializeField] private Vector2 maxPos;

    private void Awake()
    {
        player = GetComponentInParent<Player>();

    }

    private void Start()
    {
        materialHwatuUIObject = Resources.LoadAll<GameObject>("Prefabs/MaterialHwatuUI");

        blanketUI = UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[3];
        materialHwatuParent = blanketUI.transform.GetChild(0).GetComponent<RectTransform>();

        minPos = materialHwatuParent.anchoredPosition - materialHwatuParent.sizeDelta / 2;
        maxPos = materialHwatuParent.anchoredPosition + materialHwatuParent.sizeDelta / 2;

        Debug.Log(materialHwatuParent.anchoredPosition);
        Debug.Log(minPos + " , " + maxPos);
    }

    private void Update()
    {

    }

    public override void LoadEvent()
    {
        Init();
    }

    private void Init()
    {
        isDone = false;

        //모포 UI 활성화
        blanketUI.SetActive(true);

        //카드 생성
        float interval = (maxPos.x - minPos.x) / (SkillManager.instance.materialCardCnt - 1);
        for (int i=0; i < SkillManager.instance.materialCardCnt; i++)
        {
            HwatuData data1 = SkillManager.instance.materialHwatuData[i];
            for (int j = 0; j < materialHwatuUIObject.Length; j++)
            {
                HwatuData data2 = materialHwatuUIObject[j].GetComponent<MaterialHwatuSlotUI>().hwatuData;
                if (data1.hwatu.type == data2.hwatu.type)
                {
                    //화면 중앙 상단에 카드 생성
                    Vector3 initPos = new Vector3(Screen.width/2, Screen.height, 0);
                    GameObject obj = Instantiate(materialHwatuUIObject[j], initPos, Quaternion.identity, materialHwatuParent);
                    MaterialHwatuSlotUI ui = obj.GetComponent<MaterialHwatuSlotUI>();

                    //OriginPos 설정 및 이동
                    float posX = materialHwatuParent.position.x + minPos.x + interval * i;
                    //float radius = materialHwatuParent.sizeDelta.x / 2;
                    float posY = materialHwatuParent.position.y; //(Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(posX - materialHwatuParent.position.x, 2)));
                    Vector3 originPos = new Vector3(posX, posY, 0);

                    ui.pos = originPos;
                    ui.rot = Quaternion.identity;
                    ui.scale = new Vector3(2, 2, 1);
                    ui.MoveTransform(1.5f);
                    break;
                }
            }
        }
    }


}
