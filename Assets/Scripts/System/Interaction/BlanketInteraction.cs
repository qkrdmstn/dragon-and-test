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

    [Header("Prefabs")]
    [SerializeField] private GameObject[] materialHwatuUIObject; //로드되는 Prefab

    [Header("UI")]
    [SerializeField] private List<GameObject> materialHwatuUIObjectList = new List<GameObject>(); //실제 UI
    [SerializeField] private BlanketUI blanketUI;
    [SerializeField] private HwatuUI hwatuUI;

    [Header("Transform Info")]
    [SerializeField] private RectTransform materialHwatuParent; //화투 이동 위치
    [SerializeField] private Vector2 minPos;
    [SerializeField] private Vector2 maxPos;

    [Header("Data")]
    [SerializeField] private HwatuData[] selectedHwatu = new HwatuData[2];
    [SerializeField] private int selectedCnt = 0;

    private void Awake()
    {
        player = GetComponentInParent<Player>();

    }

    private void Start()
    {
        //materialUI Object Load
        materialHwatuUIObject = Resources.LoadAll<GameObject>("Prefabs/MaterialHwatuUI");

        //UI Info Init
        GameObject blanketUIObject = UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[3];
        blanketUI = blanketUIObject.GetComponent<BlanketUI>();
        GameObject hwatuUIObject = UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[5];
        hwatuUI = hwatuUIObject.GetComponent<HwatuUI>();

        //Transform Info Init
        materialHwatuParent = blanketUI.materialHwatuParent.GetComponent<RectTransform>();
        materialHwatuParent = blanketUI.materialHwatuParent.GetComponent<RectTransform>();
        minPos = materialHwatuParent.anchoredPosition - materialHwatuParent.sizeDelta / 2; //material 화투 최대/최소 위치
        maxPos = materialHwatuParent.anchoredPosition + materialHwatuParent.sizeDelta / 2;
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

        //기존 material hwatu 삭제
        for (int i = 0; i < materialHwatuUIObjectList.Count; i++)
            Destroy(materialHwatuUIObjectList[i]);
        materialHwatuUIObjectList.Clear();

        //화투 UI 비활성화 및 모포 UI 활성화
        hwatuUI.gameObject.SetActive(false);
        blanketUI.gameObject.SetActive(true);

        //화투 생성
        CreateMaterialHwatuUIObject();

        //화투 이동
        UpdateMaterialHwatuUIInitPos(1.0f);
    }

    private void CreateMaterialHwatuUIObject()
    {


        //화투 생성 위치
        RectTransform initTransform = hwatuUI.GetComponent<RectTransform>();
        
        //화투 생성
        for (int i = 0; i < SkillManager.instance.materialCardCnt; i++)
        {
            HwatuData data1 = SkillManager.instance.materialHwatuDataList[i];
            for (int j = 0; j < materialHwatuUIObject.Length; j++)
            {
                HwatuData data2 = materialHwatuUIObject[j].GetComponent<MaterialHwatuSlotUI>().hwatuData;
                if (data1.hwatu.type == data2.hwatu.type)
                {
                    //Hwatu UI에 카드 생성
                    Vector3 initPos = initTransform.position;
                    GameObject Obj = Instantiate(materialHwatuUIObject[j], initPos, Quaternion.identity, materialHwatuParent);
                    materialHwatuUIObjectList.Add(Obj);
                    break;
                }
            }
        }
    }

    private void UpdateMaterialHwatuUIInitPos(float duration)
    {
        float interval = (maxPos.x - minPos.x) / (SkillManager.instance.materialCardCnt - selectedCnt - 1);
        int j = 0;
        for (int i = 0; i < materialHwatuUIObjectList.Count; i++)
        {
            MaterialHwatuSlotUI ui = materialHwatuUIObjectList[i].GetComponent<MaterialHwatuSlotUI>();
            if (ui.isSelected) //선택된 화투패 제외
            {
                continue;
            }

            //OriginPos 설정 및 이동
            float posX = materialHwatuParent.position.x + minPos.x + interval * j;
            //float radius = materialHwatuParent.sizeDelta.x / 2;
            float posY = materialHwatuParent.position.y; //(Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(posX - materialHwatuParent.position.x, 2)));
            Vector3 originPos = new Vector3(posX, posY, 0);

            ui.originPos = originPos;
            ui.originRot = Quaternion.identity;
            ui.originScale = new Vector3(2, 2, 1);
            ui.MoveTransform(duration);

            j++;
        }
    }


    private void EndInteraction()
    {
        isDone = true;
        blanketUI.gameObject.SetActive(false);
    }

    public int AddSelectedHwatu(MaterialHwatuSlotUI ui) //실패 시, -1 반환 / 성공 시, 저장된 칸 반환
    {
        int cellNum = -1;
        HwatuData hwatuData = ui.hwatuData;

        if (selectedCnt >= 2) //빈 곳이 없을 경우, 실패
            return cellNum;
        else if(selectedCnt >= 1)
        {
            //섯다 조합 체크
            Hwatu hwatu1;
            if (selectedHwatu[0] != null)
                hwatu1 = selectedHwatu[0].hwatu;
            else
                hwatu1 = selectedHwatu[1].hwatu;
            SeotdaHwatuCombination result = Hwatu.GetHwatuCombination(hwatu1, hwatuData.hwatu);

            if (result == SeotdaHwatuCombination.blank) //섯다에 없는 조합일 경우, 실패
                return cellNum;
        }

        //앞에서부터 빈 곳에 selected hwatu data 저장
        if (selectedHwatu[0] == null)
        {
            selectedHwatu[0] = hwatuData;
            cellNum = 0;
        }
        else if (selectedHwatu[1] == null)
        {
            selectedHwatu[1] = hwatuData;
            cellNum = 1;
        }

        selectedCnt += 1;
        ui.isSelected = true;

        UpdateMaterialHwatuUIInitPos(0.5f);
        return cellNum;
    }

    public void DeleteSelectedHwatu(MaterialHwatuSlotUI ui)
    {
        HwatuData hwatuData = ui.hwatuData;
        ui.isSelected = false;

        for (int i = 0; i < 2; i++)
        {
            if(selectedHwatu[i] == hwatuData)
            {
                selectedHwatu[i] = null;
                selectedCnt -= 1;
                break;
            }
        }

        UpdateMaterialHwatuUIInitPos(0.5f);
    }

    public void CombinationSeletedHwatu()
    {
        //스킬 생성
        Hwatu.GetHwatuCombination(selectedHwatu[0].hwatu, selectedHwatu[1].hwatu);

        //사용된 material hwatu 삭제
        for (int i = 0; i < 2; i++)
            SkillManager.instance.DeleteMaterialCardData(selectedHwatu[i]);
    }
}
