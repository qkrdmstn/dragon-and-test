using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public struct DialogData
{
    public int _eventOrder;
    public string _eventName;
    public string _npcName;
    public string _dialogue;

    public bool _isSelect;
    public string[] _selectType;

    public DialogData(int eventOrder, string eventName, string npcName, string dialogue, bool isSelect, string selectType)
    {
        _eventOrder = eventOrder;
        _eventName = eventName;
        _npcName = npcName;
        _dialogue = dialogue;
        _isSelect = isSelect;

        if (selectType != null)
            _selectType = selectType.Split(',');
        else
            _selectType = null;
    }
}

public class DialogueInteraction : Interaction
{
    public int curIdx;
    public bool isFirst = true;
    public bool isSelectFirst = true;

    TextMeshProUGUI npcName;
    TextMeshProUGUI[] contentTxt;
    public TextMeshProUGUI dialogueTxt;
    public TextMeshProUGUI[] selectionTxt;

    bool keyInput, isNegative;
    public int result; // 선택된 답변의 배열 Idx
    //int count;  //현재 선택가능한 답변 갯수

    DialogueDB dialogues;
    List<DialogData> dialogDatas = new List<DialogData>(); // 현재 eventName과 동일한 대화DB를 저장할 구조체
    List<bool> selections = new List<bool>();

    public override void LoadEvent(string eventName)
    {
        UIManager.instance.SceneUI["Dialogue"].SetActive(true);
        Init();
        
        for(int i=0; i<dialogues.DialogueEntity.Count; i++)
        {
            if(dialogues.DialogueEntity[i].eventName == eventName)
            {   // 현재 이벤트에 맞는 대화DB를 저장합니다.
                dialogDatas.Add(new DialogData
                    (
                        dialogues.DialogueEntity[i].eventOrder,
                        dialogues.DialogueEntity[i].eventName,
                        dialogues.DialogueEntity[i].npcName,
                        dialogues.DialogueEntity[i].dialogue,
                        dialogues.DialogueEntity[i].isSelect,
                        dialogues.DialogueEntity[i].selectType
                    )
                );
                selections.Add(false);
            }
        }

        if (isFirst)
        {
            SetUPUI();
        }
        StartCoroutine(ManageEvent());
    }

    IEnumerator ManageEvent()
    {
        curIdx = SetNextDialog(curIdx);
        SetActiveDialogUI(true);    // 첫 대화 세팅
        yield return new WaitUntil(() => !isFirst);

        yield return new WaitUntil(() => UpdateDialogue());

        SetFalseUI();
    }

    void SetFalseUI()
    {
        UIManager.instance.SceneUI["Dialogue"].SetActive(false);

        if (GetSelectUI())
        {
            SetActiveSelectUI(false);
        }
    }

    void SetUPUI() {
        // 선택지가 2개라는 상황 종속
        npcName = UIManager.instance.SceneUI["Dialogue"].GetComponent<DialogueUIGroup>().childUI[1].GetComponent<TextMeshProUGUI>();
        contentTxt = UIManager.instance.SceneUI["Dialogue"].GetComponent<DialogueUIGroup>()
            .childUI[2].GetComponentsInChildren<TextMeshProUGUI>(true);

        dialogueTxt = contentTxt[0];
 
        selectionTxt[0] = contentTxt[1];
        selectionTxt[1] = contentTxt[2];

        selectionTxt[0].color = Color.black;
        selectionTxt[1].color = Color.black;
    }

    bool UpdateDialogue()
    {   // manage Dialogue event
        if (Input.GetKeyDown(KeyCode.Escape)) isDone = true;     // 대화 도중 나갈 수 있습니다.
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
                Debug.Log("w-0");
                result = 0;
                Selection(0);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("s-1");
                result = 1;
                Selection(1);
            }
            else if ((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) && result > -1)
            {   // 특정 경우를 선택한 경우
                Debug.Log("enter");
                keyInput = false;
                isSelectFirst = true;
                selections[curIdx] = true;  // 현 대화 분기에 대한 선택 완료

                SetActiveSelectUI(false);

                // 분기를 끝내고 다음 대화 연결
                SetActiveDialogUI(true);
                curIdx = SetNextDialog(curIdx+1);
                // TODO ------------------> 해당  선택 결과에 따른 퀘스트 시스템에 정보 연결 필요

            }
        }
        else
        {   
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
            {   //  일반 대화 출력
                if (dialogDatas.Count == curIdx) isDone = true;
                else curIdx = SetNextDialog(curIdx);
            }
        }
        
        return isDone;
    }

    void SetActiveDialogUI(bool visible)
    {   // manage dialogue UI
        if (visible) {
            // ToDo Func : Cam zoom-in
        }
        else {
            // ToDo Func : Cam zoom-out
        }

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

            if (result > -1)
            {   // 선택 결과에 따른 답변 분기 조절
                if (result == 0) idx++;                     // [예] ---- 선택에 따른 대화 지속 
                else if (result == 1) isNegative = true;    // [아니오] - 선택에 따른 대화 종료

                result = -1;
            }
            else if (!selections[idx] && dialogDatas[idx]._isSelect)
            {   // 현재 대화에 대해 선택이 필요한 경우,
                keyInput = true;
                return idx;
            }
            
            dialogueTxt.text = dialogDatas[idx]._dialogue;
            idx++;  // 다음 대화 idx 준비
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

        /*
        if (idx%2 == 0)
        { // 0번째 선택시 1번째는 반투명
            selectionTxt[1].color = color;
            color.a = 1f;
            selectionTxt[0].color = color;
        }
        else
        {   // 1번째 선택시 0번째는 반투명
            selectionTxt[0].color = color;
            color.a = 1f;
            selectionTxt[1].color = color;
        }
        */

        if(idx == 2)
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

        //result = -1;
        result = 2;
        isDone = false;
        isNegative = false;
        isFirst = true;
        keyInput = false;
        curIdx = 0;
        dialogues = UIManager.instance.dialogueDB;
        dialogDatas.Clear();
    }
}