
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialUIGroup : UIGroup
{
    public Vector3 padding;
    public Transform scarecrowPos;
    public GameObject jokbo, jokboInstantiate, testMonster, testMonsterInstantiate;
    //Animator anim;

    public bool isWASD, isAttack, isDash, isReload, isInteraction;  // variable of SetTutoTxt_1
    public bool isSkill, isOpenJokbo, isGetJokbo;   // variable of SetTutoTxt_3
    public bool isScarecrow;

    public static bool isJokbo; // 족보를 먹었는지 playerInteraction에서 체크합니다.
    public static bool isCloseJokbo;

    public float impactForce;
    #region DialogueInfo
    public int curIdx;
    public float waitTime = 0f;

    DialogueDB dialogues;
    List<DialogData> dialogDatas = new List<DialogData>(); // 현재 eventName과 동일한 대화DB를 저장할 구조체
    TextMeshProUGUI dialogueTxt;
    bool isDone;
    bool isFirst;
    #endregion

    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(scarecrowPos.position) + padding;
    }

    private void OnDisable()
    {
        isWASD = isAttack = isDash = isSkill = isReload = isInteraction = false;
    }
    void Init()
    {
        isDone = false;
        curIdx = 0;
        isFirst = true;
        isScarecrow = false;
        isJokbo = false;

        GameManager.instance.player.isAttackable = false;
    }
    void SetUPUI()
    {
        dialogueTxt = childUI[0].GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public void LoadTutorialEvent()
    {
        // 튜토리얼에 대한 대화는 여기서 따로 관리하는 식으로 진행 <-> dialogueInteraction
        dialogues = UIManager.instance.dialogueDB;
        
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

        if(isFirst)
            SetUPUI();

        StartCoroutine(ManageEvent());
    }
    IEnumerator ManageEvent()
    {
        yield return new WaitForSeconds(1.5f);
        ToggleUI(childUI[0].gameObject);
        yield return new WaitUntil(() => SetTutoTxt_1());

        yield return new WaitUntil(() => SetTutoTxt_3());

        ToggleUI(childUI[0].gameObject);
    }

   void SetNextDialog(int idx)
    {
        if (dialogDatas.Count > idx)
        {
            dialogueTxt.text = dialogDatas[idx]._dialogue;
        }
    }

    float time = 0f;
    bool SetTutoTxt_1()
    {
        SetNextDialog(curIdx);

        switch (curIdx)
        {
            case 0: // welcome
                time += Time.deltaTime;
                if (time > waitTime)
                {   // 3초의 대기시간 이후,
                    Debug.Log("0-welcome");
                    curIdx++;
                    GameManager.instance.player.isAttackable = true;
                }
                break;
            case 1: // move
                if (CheckState("Move"))
                {
                    curIdx++;
                    Debug.Log("1-move");
                }
                break;
            case 2:
                if (CheckState("AttacknDash"))
                {
                    curIdx++;
                    Debug.Log("2-AttacknDash");
                }
                break;
            case 3:
                if (CheckState("Reload"))
                {
                    curIdx++;
                    Debug.Log("3-reload");
                }
                break;
            case 4:
                if (CheckState("Interaction"))
                {
                    curIdx++;
                    Debug.Log("4-interaction");
                }
                break;
            default:    // 이외 일반 대화 출력
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (curIdx > 9)
                    {
                        return true;
                    }
                    curIdx++;
                }
                break;
        }

        return false;
    }
    bool SetTutoTxt_3()
    {
        SetNextDialog(curIdx);

        switch (curIdx)
        {
            case 11: // GetJokbo - F
                if (jokboInstantiate == null)
                {
                    jokboInstantiate = Instantiate(jokbo,
                    (GameManager.instance.player.transform.position + Vector3.left),
                    Quaternion.identity, scarecrowPos.parent);
                }
                else if (CheckState("GetJokbo"))
                {
                    curIdx++;
                    Debug.Log("12 - isGetJokbo");
                }
                break;
            case 12: // OpenJokbo
                // 족보 설명 UI 열기
                if (CheckState("OpenJokbo"))
                {
                    curIdx++;
                    Debug.Log("13 - isOpenJokbo");
                }
                break;
            case 16: // UseSkill - knockback
                if (testMonsterInstantiate == null)
                {
                    testMonsterInstantiate = Instantiate(testMonster,
                    (GameManager.instance.player.transform.position + (Vector3.right*2f)),
                    Quaternion.identity, scarecrowPos.parent);
                }
                else if (CheckState("Skill"))
                {
                    curIdx++;
                    Debug.Log("17 - isSkill");
                }
                break;
            default:    // 이외 일반 대화 출력
                if (Input.GetKeyDown(KeyCode.F))
                {
                    curIdx++;
                    if (dialogDatas.Count == curIdx) isDone = true;
                }
                break;
        }
        return isDone;   // true : 대화가 종료되어 해당 함수의 호출을 멈춤. false : 대화가 아직 종료되지 않아 해당 함수의 호출을 계속함.
    }

    bool CheckState(string curType)
    {  
        if (!isWASD && curType == "Move" && (Input.GetKeyDown(KeyCode.W)
            || Input.GetKeyDown(KeyCode.A)
            || Input.GetKeyDown(KeyCode.S)
            || Input.GetKeyDown(KeyCode.D)))
        {
            isWASD = true;
            return isWASD;
        }

        else if (isWASD && !isDash && Input.GetKeyDown(KeyCode.Mouse1) && GameManager.instance.player.IsDash())
        {
            isDash = true;
            Debug.Log("isDash");
        }

        else if (isAttack && isDash && curType == "AttacknDash")
        {   // isAttack은 ScarecrowAttacked에서 상태확인
            Debug.Log("isAttacknDash");
            return isAttack | isDash;
        }

        else if (!isReload && curType == "Reload" && Input.GetKeyDown(KeyCode.R))
        {
            isReload = true;
            Debug.Log("isReload");
            return isReload;
        }

        else if(!isInteraction && isScarecrow && curType == "Interaction" && Input.GetKeyDown(KeyCode.F))
        {
            // 허수아비에게 가서 F키를 눌렀는가 체크
            isInteraction = true;
            Debug.Log("isInteraction");
            return isInteraction;
        }

        // ---------- tuto3 : 섯다 족보 소개
        else if(!isGetJokbo && isJokbo && curType == "GetJokbo")
        {
            Debug.Log("isGetJokbo");
            return isGetJokbo = true;
        }

        else if(!isOpenJokbo && curType=="OpenJokbo" && Input.GetKeyDown(KeyCode.K))
        {   // UI가 닫혔는지 JolboUIGroup에서 상태확인
            if(isCloseJokbo) isOpenJokbo = true;
            return isOpenJokbo;
        }

        else if (!isSkill && curType == "Skill" && Input.GetKeyDown(KeyCode.Q))
        {    
            Vector2 impactDir = testMonsterInstantiate.transform.position - GameManager.instance.player.transform.position;
            impactDir.Normalize();

            testMonsterInstantiate.GetComponent<MonsterTutorial>().Knockback(impactDir, impactForce);
            //testMonsterInstantiate.SetActive(false);

            return isSkill = true;
        }

        return false;
    }

    //IEnumerator CheckStateCouroutine(string curType)
    //{   // UI 활성화 애니메이션이 끝나면 호출되는 코루틴, UI 비활성화 ON
    //    yield return new WaitUntil(() => CheckState(curType));

    //    if (curType == "Reload" && isReload)
    //    {
    //        anim.SetBool("isReload", false);
    //    }
    //    else if(curType == "Interaction" && isInteraction)
    //    {
    //        anim.SetBool("isInteraction", false);
    //    }
    //    else if (curType == "Skill" && isSkill)
    //    {
    //        anim.SetBool("isSkill", false);
    //        anim.SetBool("isInteraction", true);
    //        //anim.SetBool("isReload", true);
    //    }
    //    else if (curType == "Move" && isWASD)
    //    {
    //        GameManager.instance.player.isAttackable = true;
    //        anim.SetBool("isWASD", false);
    //        anim.SetBool("isSkill", true);
    //    }
    //}
}
