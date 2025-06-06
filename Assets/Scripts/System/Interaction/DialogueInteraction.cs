using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using TMPro;

public class DialogueInteraction : Interaction
{
    public int curIdx;
    public bool isFirst = true;
    public bool isSelectFirst = true;

    UnityEngine.UI.Image npcImg;
    TextMeshProUGUI npcName;
    TextMeshProUGUI[] contentTxt;
    public TextMeshProUGUI dialogueTxt;
    public TextMeshProUGUI[] selectionTxt;

    bool keyInput, isNegative;
    public int result; // 선택된 답변의 배열 Idx

    DialogueDBEntity[] dialogues;                           // 전체 대화목록
    List<DialogData> dialogDatas = new List<DialogData>();  // 현재 eventName과 동일한 대화목록
    List<bool> selections = new List<bool>();
    InteractionData data;
    DialogueUIGroup dialogueUIGroup;

    private async void Start()
    {
        dialogueUIGroup = UIManager.instance.GetComponentInChildren<DialogueUIGroup>(true);
        await LoadDialogueDBEntity();
    }

    public override void LoadEvent(InteractionData data)
    {
        this.data = data;

        UIManager.instance.PushPopUI(UIManager.instance.SceneUI["Dialogue"]);
        Init();
        LoadDialogData();

        if (isFirst) SetUPUI();
        StartCoroutine(ManageEvent());
    }

    async Task LoadDialogueDBEntity()
    {
        dialogues = await DataManager.instance.GetValues<DialogueDBEntity>(SheetType.Dialog, "A1:F");
        ScenesManager.instance.isLoadedDB++;
    }

    void LoadDialogData()
    {
        for (int i = 0; i < dialogues.Length; i++)
        {
            if (dialogues[i].eventName == data.eventName)
            {   // 현재 이벤트에 맞는 대화DB를 저장
                dialogDatas.Add(new DialogData
                    (
                        dialogues[i].eventOrder,
                        dialogues[i].eventName,
                        dialogues[i].npcName,
                        dialogues[i].dialogue,
                        dialogues[i].isSelect,
                        dialogues[i].selectType
                    )
                );
                selections.Add(false);
            }
        }
    }

    IEnumerator ManageEvent()
    {
        curIdx = SetNextDialog(curIdx);
        SetActiveDialogUI(true);    // 첫 대화 세팅
        yield return new WaitUntil(() => !isFirst);

        yield return new WaitUntil(() => UpdateDialogue());

        SetFalseUI();
    }

    public void SetFalseUI()
    {
        if (dialogueUIGroup.isExit)
        {
            dialogueUIGroup.isExit = false;
        }
        if (GetSelectUI())
        {
            SetActiveSelectUI(false);
        }
        UIManager.instance.isClose = true;
    }

    void SetUPUI() {
        // 선택지가 2개라는 상황 종속
        npcImg = UIManager.instance.SceneUI["Dialogue"].GetComponent<DialogueUIGroup>()
            .childUI[0].GetComponent<UnityEngine.UI.Image>();
        npcName = UIManager.instance.SceneUI["Dialogue"].GetComponent<DialogueUIGroup>()
            .childUI[1].GetComponent<TextMeshProUGUI>();
        contentTxt = UIManager.instance.SceneUI["Dialogue"].GetComponent<DialogueUIGroup>()
            .childUI[2].GetComponentsInChildren<TextMeshProUGUI>(true);

        npcImg.sprite = data.npcImg;
        dialogueTxt = contentTxt[0];
 
        selectionTxt[0] = contentTxt[1];
        selectionTxt[1] = contentTxt[2];

        selectionTxt[0].color = Color.black;
        selectionTxt[1].color = Color.black;
    }
    
    bool UpdateDialogue()
    {   // manage Dialogue event
        if (dialogueUIGroup.isExit || Input.GetKeyDown(KeyCode.Escape)) isDone = true;     // 버튼으로 대화 도중 나갈 수 있습니다.
        if (isDone)  SetActiveDialogUI(false);

        if (keyInput)
        {   // 선택 대화 출력
            if(result == -1) Selection(2);  // 처음 선택에 대한 디폴트 선택을 지정합니다. 이후 방향키 입력이 있다면, 발동되지 않습니다
            SetNextSelection();

            if (isSelectFirst)
            {
                SetActiveSelectUI(true);
                SetActiveDialogUI(false);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                result = 0;
                Selection(0);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                result = 1;
                Selection(1);
            }
            else if ((Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0)) && result > -1)
            {   // 특정 경우를 선택한 경우
                keyInput = false;
                isSelectFirst = true;
                selections[curIdx] = true;  // 현 대화 분기에 대한 선택 완료
                SetActiveSelectUI(false);

                // 분기를 끝내고 다음 대화 연결
                SetActiveDialogUI(true);
                curIdx = SetNextDialog(curIdx + 1);
            }
        }
        else
        {   
            if (Input.GetKeyDown(KeyCode.F))
            {   //  일반 대화 출력
                SoundManager.instance.SetEffectSound(SoundType.UI, UISfx.Dialogue);
                if (dialogDatas.Count == curIdx)
                {
                    SoundManager.instance.SetEffectSound(SoundType.NPC, data.sequence+1);
                    isDone = true;
                }
                else curIdx = SetNextDialog(curIdx);
            }
        }
        
        return isDone;
    }

    public void SetActiveDialogUI(bool visible)
    {   
        dialogueTxt.gameObject.SetActive(visible);
        
        if (isFirst) isFirst = false;
    }

    void SetActiveSelectUI(bool visible)
    {   // manage select UI
        selectionTxt[0].gameObject.SetActive(visible);
        selectionTxt[1].gameObject.SetActive(visible);
    }
    bool GetSelectUI()
    {
        return selectionTxt[0].gameObject.activeSelf;
    }

    int SetNextDialog(int idx)
    {
        if (isNegative) isDone = true;
        else if (dialogDatas.Count > idx)
        {
            npcName.text = dialogDatas[idx]._npcName;

            if (isFirst)
            {   // 첫 대화 출력
                dialogueTxt.text = dialogDatas[idx]._dialogue;
                SoundManager.instance.SetEffectSound(SoundType.NPC, data.sequence);
                if (!dialogDatas[idx]._isSelect) idx++;
            }
            else if (result > -1)
            {   // 선택 결과에 따른 답변 분기 조절
                switch (result)
                {
                    case 0:     // [예] ---- 선택에 따른 대화 지속    
                        idx++; 
                        break;
                    case 1:     // [아니오] - 선택에 따른 대화 종료
                        isNegative = true;   
                        break;
                }
                result = -1;
                dialogueTxt.text = dialogDatas[idx]._dialogue;
                idx++;  // 다음 대화 idx 준비
            }
            else if (!selections[idx] && dialogDatas[idx]._isSelect)
            {   // 현재 대화에 대해 선택이 필요한 경우,
                keyInput = true;
            }
            else
            {   // 이외 일반 대화 출력
                dialogueTxt.text = dialogDatas[idx]._dialogue;
                idx++;
            }
        }
        return idx;
    }

    void SetNextSelection()
    {
        for(int i = 0; i < selectionTxt.Length; i++)
        {
            selectionTxt[i].text = dialogDatas[curIdx]._selectType[i];
        }
    }

    void Selection(int idx)
    {   // input = 선택한 분기의 인덱스 : w - 0 / s - 1
        Color color = selectionTxt[0].color;
        color.a = 0.25f;

        selectionTxt[(idx+1) % 2].color = color;    // 반투명
        color.a = 1f;
        selectionTxt[idx % 2].color = color;
        SoundManager.instance.SetEffectSound(SoundType.UI, UISfx.Select);

        if (idx == 2)
        {
            result = 0;
        }
    }

    void Init()
    {
        selectionTxt = new TextMeshProUGUI[2];
        for(int i=0; i<selections.Count; i++)
        {
            selections[i] = false;
        }

        result = -1;
        curIdx = 0;
        isFirst = true;

        isDone = false;
        isNegative = false;
        keyInput = false;

        dialogDatas.Clear();
    }
}