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
    public bool isSelecting = false;
    public List<TextMeshProUGUI> txt = new List<TextMeshProUGUI>();
    //TextMeshProUGUI npcName, dialogTxt;
    GameObject[] dialogueChild;

    DialogueDB dialogues;
    List<DialogData> dialogDatas = new List<DialogData>(); // 현재 eventName과 동일한 대화DB를 저장할 구조첸

    public override void LoadEvent(string eventName)
    {
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
            }
        }

        if (isFirst)
        {
            SetUPUI();
        }
        SetNextDialog();

        StartCoroutine(ManageEvent());
    }

    IEnumerator ManageEvent()
    {
        yield return new WaitUntil(() => UpdateDialogue());
    }

    void SetUPUI() {
        SetActiveDialogUI(true);

        dialogueChild = UIManager.instance.SceneUI["Dialogue"].GetComponent<DialogueUIGroup>().childUI;
        //TextMeshProUGUI[] txtUI = UIManager.instance.SceneUI["Dialogue"].GetComponentsInChildren<TextMeshProUGUI>();

        foreach(GameObject ele in dialogueChild)
        {
            TextMeshProUGUI[] tmp = ele.GetComponentsInChildren<TextMeshProUGUI>();
            if (tmp != null) {
                if (tmp.Length == 1) txt.Add(tmp[0]);
                else {
                    foreach(TextMeshProUGUI i in tmp) {
                        txt.Add(i);     // 선택 텍스트 저장
                    }
                }
            }
        }
    }

    bool UpdateDialogue()
    {   // manage Dialogue event
        //if (isSelecting && Input.GetMouseButtonDown(0))
        //{
        //    SetNextSelect();

        //    if (Input.GetKeyDown(KeyCode.W))
        //    {
        //        txt[2].color = Color.red;
        //        txt[3].color = Color.cyan;
        //        // 선택에 따른 처리

        //        isSelecting = false;
        //        SetActiveSelectUI(isSelecting);
        //    }
        //    else if (Input.GetKeyDown(KeyCode.S))
        //    {
        //        txt[2].color = Color.cyan;
        //        txt[3].color = Color.red;

        //        isSelecting = false;
        //        SetActiveSelectUI(isSelecting);
        //    }
        //}
        if (Input.GetMouseButtonDown(0))
        {
            if (dialogDatas.Count > curIdx + 1)
            {   // 다음으로 출력할 대화가 존재하는 경우,
                SetNextDialog();
            }
            else
            {
                SetActiveDialogUI(false);
                isDone = true;
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

        UIManager.instance.SceneUI["Dialogue"].SetActive(visible);
    }

    void SetActiveSelectUI(bool visible)
    {   // manage select UI
        if (visible)
        {   // 선택의 경우 
            dialogueChild[2].SetActive(false);
            dialogueChild[3].SetActive(true);
        }
        else
        {   // 선택이 종료되어 다시 대화창이 나와야하는 경우
            dialogueChild[3].SetActive(false);
            dialogueChild[2].SetActive(true);

            curIdx++;
        }
    }

    void SetNextDialog()
    {
        curIdx++;

        txt[0].text = dialogDatas[curIdx]._npcName;
        txt[1].text = dialogDatas[curIdx]._dialogue;
    }

    void SetNextSelect()
    {
        txt[2].text = dialogDatas[curIdx]._selectType[0];
        txt[3].text = dialogDatas[curIdx]._selectType[1];
    }

    void Init()
    {
        isDone = false;
        curIdx = -1;
        dialogues = UIManager.instance.dialogueDB;
        dialogDatas.Clear();
        txt.Clear();
    }
}