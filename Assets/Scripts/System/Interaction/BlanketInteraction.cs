using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlanketInteraction : Interaction
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] materialHwatuUIObject; //로드되는 Prefab

    [Header("UI")]
    [SerializeField] private List<GameObject> materialHwatuUIObjectList = new List<GameObject>(); //실제 UI
    [SerializeField] private BlanketUI blanketUI;

    [Header("Transform Info")]
    [SerializeField] private RectTransform materialHwatuParent; //화투 이동 위치
    [SerializeField] private Vector2 minPos;
    [SerializeField] private Vector2 maxPos;

    [Header("Data")]
    [SerializeField] private MaterialHwatuSlotUI[] selectedHwatuUI = new MaterialHwatuSlotUI[2];
    public int selectedCnt = 0;

    [Header("State")]
    public bool isBlanketInteraction;

    private void Start()
    {
        //materialUI Object Load
        materialHwatuUIObject = Resources.LoadAll<GameObject>("Prefabs/MaterialHwatuUI");

        //UI Info Init
        //GameObject hwatuUIObject = UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[4];
        GameObject blanketUIObject = UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[5];
        blanketUI = blanketUIObject.GetComponent<BlanketUI>();

        //Transform Info Init
        materialHwatuParent = blanketUI.materialHwatuParent.GetComponent<RectTransform>();
        minPos = materialHwatuParent.anchoredPosition - materialHwatuParent.sizeDelta / 2; //material 화투 최대/최소 위치
        maxPos = materialHwatuParent.anchoredPosition + materialHwatuParent.sizeDelta / 2;
    }

    private void Update()
    {
        if (!blanketUI.isSkillInfoUI && isBlanketInteraction && Input.GetKeyDown(KeyCode.Escape))
        {
            if (Player.instance.isTutorial && !TutorialInteraction.isBlanket) return;

            if (UIManager.instance.curOpenUI.Count == 1)
            {
                UIManager.instance.isClose = true;
                EndInteraction();
            }
                
        }
        else if (blanketUI.isSkillInfoUI && Input.GetKeyDown(KeyCode.Escape))
        {
            blanketUI.SetSkillInfoUIInActive();
        }
    }

    public override void LoadEvent()
    {
        Init();
    }

    private void Init()
    {
        isBlanketInteraction = true;
        isDone = false;
        SoundManager.instance.SetEffectSound(SoundType.UI, UISfx.mopo);

        //기존 material hwatu 삭제
        for (int i = 0; i < materialHwatuUIObjectList.Count; i++)
            Destroy(materialHwatuUIObjectList[i]);
        materialHwatuUIObjectList.Clear();
        selectedCnt = 0;

        //화투 UI 비활성화 및 모포 UI 활성화
        //hwatuUI.gameObject.SetActive(false);
        UIManager.instance.SceneUI["Inventory"].SetActive(false);
        UIManager.instance.PushPopUI(blanketUI.gameObject);

        //화투 생성
        CreateMaterialHwatuUIObject();

        //화투 이동
        UpdateMaterialHwatuUIInitPos(1.0f);
    }

    private void CreateMaterialHwatuUIObject()
    {
        //화투 생성 위치
        //RectTransform initTransform = blanketUI.GetComponent<RectTransform>();

        //화투 생성
        for (int i = 0; i < SkillManager.instance.refMaterialCardCnt; i++)
        {
            HwatuData data1 = SkillManager.instance.materialHwatuDataList[i];
            for (int j = 0; j < materialHwatuUIObject.Length; j++)
            {
                HwatuData data2 = materialHwatuUIObject[j].GetComponent<MaterialHwatuSlotUI>().hwatuData;
                if (data1.hwatu.type == data2.hwatu.type)
                {
                    //Hwatu UI에 카드 생성
                    Vector3 initPos = materialHwatuParent.position;
                    GameObject Obj = Instantiate(materialHwatuUIObject[j], initPos, Quaternion.identity, materialHwatuParent);
                    materialHwatuUIObjectList.Add(Obj);
                    break;
                }
            }
        }
    }

    private void UpdateMaterialHwatuUIInitPos(float duration)
    {
        int numOfCard = 9; //최대 카드 - 모포 위의 카드 - 1
        float interval = (maxPos.x - minPos.x) / (float)numOfCard;
        int j = 0;
        for (int i = 0; i < materialHwatuUIObjectList.Count; i++)
        {
            MaterialHwatuSlotUI ui = materialHwatuUIObjectList[i].GetComponent<MaterialHwatuSlotUI>();
            if (ui.isSelected) //선택된 화투패 제외
                continue;

            //OriginPos 설정 및 이동
            float posX = materialHwatuParent.position.x + minPos.x + interval * j;
            if (numOfCard == 0)
                posX = materialHwatuParent.position.x;

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

    public void EndInteraction()
    {
        isDone = true;
        isBlanketInteraction = false;

        //hwatuUI.gameObject.SetActive(true);
        UIManager.instance.SceneUI["Inventory"].SetActive(true);
    }

    public bool AddSelectedHwatu(MaterialHwatuSlotUI ui)
    {
        HwatuData hwatuData = ui.hwatuData;

        if (selectedCnt >= 2) //화투 select에 빈 곳이 없을 경우, 실패
            return false;
        else if (selectedCnt >= 1)
        {
            //섯다 조합 체크
            Hwatu hwatu1;
            if (selectedHwatuUI[0] != null)
                hwatu1 = selectedHwatuUI[0].hwatuData.hwatu;
            else
                hwatu1 = selectedHwatuUI[1].hwatuData.hwatu;
            SeotdaHwatuCombination result = Hwatu.GetHwatuCombination(hwatu1, hwatuData.hwatu);

            if (result == SeotdaHwatuCombination.blank) //섯다에 없는 조합일 경우, 실패
                return false;
            else if(SkillManager.instance.IsFullActive() && !SkillManager.instance.IsPassive(result)) //액티브 가득 참 & 현재 조합이 액티브일 경우
                return false;
        }

        //앞에서부터 빈 곳에 selected hwatu data 저장
        if (selectedHwatuUI[0] == null)
        {
            selectedHwatuUI[0] = ui;
        }
        else if (selectedHwatuUI[1] == null)
        {
            selectedHwatuUI[1] = ui;
        }

        ui.isSelected = true;
        selectedCnt++;

        UpdateMaterialHwatuUIInitPos(0.5f);

        if (selectedCnt == 2)
        {
            Invoke("CombinationSeletedHwatu", 0.2f); //화투패 UI 작아지는 애니메이션 후, 조합 실행
        }
        return true;
    }

    public void CancelSelectedHwatu()
    {
        for (int i = 0; i < 2; i++)
        {
            if (selectedHwatuUI[i] != null)
            {
                selectedHwatuUI[i].isSelected = false;
                selectedHwatuUI[i] = null;
                selectedCnt -= 1;
            }
        }
        UpdateMaterialHwatuUIInitPos(0.5f);
    }

    public void DeleteHwatu(MaterialHwatuSlotUI ui)
    {
        SkillManager.instance.DeleteMaterialCardData(ui.hwatuData);
        materialHwatuUIObjectList.Remove(ui.gameObject);
        Destroy(ui.gameObject);

        UpdateMaterialHwatuUIInitPos(0.5f);
    }

    public void CombinationSeletedHwatu()
    {
        //스킬 생성
        SeotdaHwatuCombination result = Hwatu.GetHwatuCombination(selectedHwatuUI[0].hwatuData.hwatu, selectedHwatuUI[1].hwatuData.hwatu);
        SkillManager.instance.AddSkill(result);

        //사용된 material hwatu 삭제
        for (int i = 0; i < 2; i++)
        {
            SkillManager.instance.DeleteMaterialCardData(selectedHwatuUI[i].hwatuData);
            materialHwatuUIObjectList.Remove(selectedHwatuUI[i].gameObject);
            Destroy(selectedHwatuUI[i].gameObject);
            selectedHwatuUI[i] = null;
        }
        selectedCnt = 0;

        blanketUI.SetSkillInfoUIActive(result);
    }
}
