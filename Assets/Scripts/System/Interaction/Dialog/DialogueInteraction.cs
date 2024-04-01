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

    public DialogData(int eventOrder, string eventName, string npcName, string dialogue)
    {
        _eventOrder = eventOrder;
        _eventName = eventName;
        _npcName = npcName;
        _dialogue = dialogue;
    }
}

public class DialogueInteraction : Interaction
{
    int curDialogIdx;
    bool isFirst = true;
    TextMeshProUGUI npcName, dialogTxt;

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
                        dialogues.DialogueEntity[i].dialogue
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

        TextMeshProUGUI[] txtUI = UIManager.instance.SceneUI["Dialogue"].GetComponentsInChildren<TextMeshProUGUI>();
        npcName = txtUI[0];
        dialogTxt = txtUI[1];
    }

    bool UpdateDialogue()
    {   // manage Dialogue event
        if (Input.GetMouseButtonDown(0))
        {
            if (dialogDatas.Count > curDialogIdx + 1)
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

    void SetNextDialog()
    {
        curDialogIdx++;

        npcName.text = dialogDatas[curDialogIdx]._npcName;
        dialogTxt.text = dialogDatas[curDialogIdx]._dialogue;
    }

    void Init()
    {
        isDone = false;
        curDialogIdx = -1;
        dialogues = UIManager.instance.dialogueDB;
        dialogDatas.Clear();
    }
}