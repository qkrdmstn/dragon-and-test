using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIGroup : UIGroup
{
    public Vector3 padding;
    Animator anim;

    public bool isWASD, isAttack, isDash, isSkill, isReload, isInteraction; // UI가 뜨고 플레이어가 해당 UI에 대한 key를 누르면 활성화

    #region DialogueInfo
    public int curIdx;
    DialogueDB dialogues;
    List<DialogData> dialogDatas = new List<DialogData>(); // 현재 eventName과 동일한 대화DB를 저장할 구조체
    TextMeshProUGUI npcName;
    TextMeshProUGUI dialogueTxt;
    bool isDone, isFirst;
    #endregion

    private void Awake()
    {
        padding = new Vector3(0, 175f, 0);
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (anim == null) return;

        anim.enabled = true;
    }

    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(GameManager.instance.player.transform.position) + padding;
    }

    private void OnDisable()
    {
        isWASD = isAttack = isDash = isSkill = isReload = isInteraction = false;

        anim.Rebind();
        anim.enabled = false;
    }
    void Init()
    {
        isDone = false;
        isFirst = true;
        curIdx = 0;
        GameManager.instance.player.isAttackable = false;
    }
    void SetUPUI()
    {
        npcName = UIManager.instance.SceneUI["Dialogue"].GetComponent<DialogueUIGroup>().childUI[1].GetComponent<TextMeshProUGUI>();
        dialogueTxt = UIManager.instance.SceneUI["Dialogue"].GetComponent<DialogueUIGroup>()
            .childUI[2].GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public void LoadTutorialEvent()
    {
        // 튜토리얼에 대한 대화는 여기서 따로 관리하는 식으로 진행 <-> dialogueInteraction
        dialogues = UIManager.instance.dialogueDB;
        UIManager.instance.SceneUI["Dialogue"].SetActive(true);
        Init();

        for (int i = 0; i < dialogues.DialogueEntity.Count; i++)
        {
            if (dialogues.DialogueEntity[i].eventName.Contains("튜토리얼"))
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
        StartCoroutine(ManageEvent());
    }
    IEnumerator ManageEvent()
    {
        curIdx = SetNextDialog(curIdx);
        SetActiveDialogUI(true);    // 첫 대화 세팅
        yield return new WaitUntil(() => !isFirst);

        //if (!isWASD && UIManager.instance.fade.fadePanel.color.a < 0.1) anim.SetBool("isWASD", true);

        yield return new WaitUntil(() => UpdateDialogue());

        UIManager.instance.SceneUI["Dialogue"].SetActive(false);
    }
    bool UpdateDialogue()
    {   
        if (Input.GetKeyDown(KeyCode.Escape)) isDone = true;     // 대화 도중 나갈 수 있습니다.
        if (isDone) SetActiveDialogUI(false);
        else
        {
            if (Input.GetKeyDown(KeyCode.F))
            {   //  일반 대화 출력
                if (dialogDatas.Count == curIdx) isDone = true;
                else curIdx = SetNextDialog(curIdx);
            }
        }

        return isDone;
    }
    void SetActiveDialogUI(bool visible)
    {   // manage dialogue UI
        dialogueTxt.gameObject.SetActive(visible);
        if (isFirst) isFirst = false;
    }
    int SetNextDialog(int idx)
    {
        if (dialogDatas.Count > idx)
        {
            npcName.text = dialogDatas[idx]._npcName;

            if (isFirst)
            {   // 첫 대화 출력
                dialogueTxt.text = dialogDatas[idx]._dialogue;
                if (!dialogDatas[idx]._isSelect) idx++;
            }
            
            else
            {   // 이외 일반 대화 출력 ---> 분기에 맞게 조건화 필요
                dialogueTxt.text = dialogDatas[idx]._dialogue;
                idx++;
            }
        }
        return idx;
    }
    IEnumerator CheckStateCouroutine(string curType)
    {   // UI 활성화 애니메이션이 끝나면 호출되는 코루틴, UI 비활성화 ON
        yield return new WaitUntil(() => CheckState(curType));

        if (curType == "Reload" && isReload)
        {
            anim.SetBool("isReload", false);
        }
        else if(curType == "Interaction" && isInteraction)
        {
            anim.SetBool("isInteraction", false);
        }
        else if (curType == "Skill" && isSkill)
        {
            anim.SetBool("isSkill", false);
            anim.SetBool("isInteraction", true);
            //anim.SetBool("isReload", true);
        }
        else if (curType == "Move" && isWASD)
        {
            GameManager.instance.player.isAttackable = true;
            anim.SetBool("isWASD", false);
            anim.SetBool("isSkill", true);
        }
    }

    public bool CheckState(string curType)
    {   // 현재 진행 UI에 따라서 다음 애니메이션을 준비
        if (!isWASD && curType == "Move" && (Input.GetKeyDown(KeyCode.W)
            || Input.GetKeyDown(KeyCode.A)
            || Input.GetKeyDown(KeyCode.S)
            || Input.GetKeyDown(KeyCode.D)))
        {
            isWASD = true;
            return isWASD;
        }

        else if (isWASD && !isAttack && Input.GetKeyDown(KeyCode.Mouse0))
        {
            isAttack = true;
            Debug.Log("isAttack");
        }

        else if (isWASD && !isDash && Input.GetKeyDown(KeyCode.Mouse1) && GameManager.instance.player.IsDash())
        {
            isDash = true;
            Debug.Log("isDash");
        }

        else if (isAttack && isDash && curType == "Skill")
        {
            isSkill = true;
            Debug.Log("isSkill");
            return isSkill;
        }

        else if (!isReload && curType == "Reload" && Input.GetKeyDown(KeyCode.R))
        {
            isReload = true;
            Debug.Log("isReload");
            return isReload;
        }

        else if(!isInteraction && curType == "Interaction" && Input.GetKeyDown(KeyCode.F))
        {
            isInteraction = true;
            Debug.Log("isInteraction");
            return isInteraction;
        }

        return false;
    } 
}
