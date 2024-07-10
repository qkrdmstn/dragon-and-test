using Google.Apis.Sheets.v4.Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ScareScrowType
{
    Start,
    AttackDash,
    Reload,
    MiniBattle,
    Jokbo,
    Skill
}
public enum TutorialUIListOrder
{
    Dialogue,
    Interation,
    BasicSkill,
    Reload,
    OpenJokbo,
    HwatuSkill,
    Move
}

public class Tutorial : MonoBehaviour
{
    [SerializeField] List<GameObject> scareScrows;
    [SerializeField] List<BoxCollider2D> boundColliders;
    [SerializeField] GameObject monsters;
    [SerializeField] GameObject jokbo;
    [SerializeField] GameObject blanket;

    List<TutorialDBEntity> tutoDB = new List<TutorialDBEntity>();

    public Vector3 padding;
    GameObject curScarescrow = null;
    BoxCollider2D curBoundCollider = null;
    PlayerInteraction playerInteraction;

    delegate bool OnTutorials();
    OnTutorials onTutorials;

    bool OccurTutorial(OnTutorials tutoFunction)
    {
        return tutoFunction();
    }

    # region DialogInteractionInfo
    List<TutorialData> tutorialDatas = new List<TutorialData>();
    struct TutorialData {
        // sequence별로 대화를 저장하는 구조체
        public int sequence;
        public List<DialogAction> dialogues;

        public TutorialData(int _sequence, List<DialogAction> _dialogues)
        {
            sequence = _sequence;
            dialogues = _dialogues;
        }
    }

    struct DialogAction
    {
        public string dialog;
        public bool isAction;

        public DialogAction(string _dialog, bool _isAction)
        {
            dialog = _dialog;
            isAction = _isAction;
        }
    }
    #endregion

    // DialogUI
    TextMeshProUGUI dialogTxt;

    // CheckVariable
    public bool canSpeak = false;
    public bool isInteraction = false;
    public bool isActiveDone = false;
    public int curSequence;
    public int curIdx = 0;
    public static bool generateBullet = false;

    public bool[] checkSequenceDone;

    private void Start()
    {
        LoadTutorialDBEntity();
        LoadDialog();
        checkSequenceDone = new bool[tutorialDatas.Count];

        Invoke("StartFirstDialog", 1.5f);
    }

    private void Update()
    {
        if (curSequence < 0) return;
        if (dialogTxt == null)
        {
            dialogTxt = UIManager.instance.curUIGroup.childUI[0].GetComponentInChildren<TextMeshProUGUI>(true);
        }

        if(curScarescrow != null)
        {   // 허수아비의 머리 위에 말풍선을 위치
            UIManager.instance.curUIGroup.childUI[0].transform.position = 
                Camera.main.WorldToScreenPoint(curScarescrow.transform.position) + padding;
        }

        if ((canSpeak && Input.GetKeyDown(KeyCode.F)) || isActiveDone)
        {   // 일반 대화 출력
            if (isActiveDone) { isActiveDone = false; canSpeak = true; }

            SetNextDialog();
            if (curIdx >= tutorialDatas[curSequence].dialogues.Count)
            {
                IsDone();
            }
            else curIdx++;
        }
        else if (curSequence > 0 && isInteraction)
        {   // 현재 대화에 대한 이벤트 호출
            if (OccurTutorial(onTutorials))
            {
                isActiveDone = true;
                UIManager.instance.curUIGroup.AttachUIforPlayer(-1);
            }
        }
    }

    public GameObject GetCurScarescrow() => curScarescrow;

    void LoadTutorialDBEntity()
    {
        IList<IList<object>> data = DataManager.instance.SelectDatas(SheetType.Tutorial, "Tutorial");
        for (int i = 1; i < data.Count; i++)
        {
            TutorialDBEntity tmp = DataManager.instance.SplitContextTutorial(data[i]);
            tutoDB.Add(tmp);
        }
    }

    void LoadDialog()
    {   // 튜토리얼 대화를 시퀀스별로 불러와 저장합니다.
        int curSequence = -1;
        for (int i = 0; i < tutoDB.Count; i++)
        {
            curSequence++;
            List<DialogAction> tmpDialogs = new List<DialogAction>();
            while (curSequence == tutoDB[i].sequence)
            {
                tmpDialogs.Add(new DialogAction(tutoDB[i].dialogue, tutoDB[i].isInteraction));
                if (tutoDB.Count == i+1 || curSequence != tutoDB[i + 1].sequence)
                {
                    tutorialDatas.Add(new TutorialData(curSequence, tmpDialogs));
                    break;
                }
                else i++;
            }
        }
    }

    void StartFirstDialog()
    {
        curSequence = 0;
        SetScarescrow(ScareScrowType.Start);

        playerInteraction = GameManager.instance.player.GetComponentInChildren<PlayerInteraction>();
        StartCoroutine(ManageDialog());
    }

    public void LoadEvent(int sequence)
    {   // player가 상호작용한 허수아비에 맞춰 이벤트가 진행됩니다.
        SetScarescrow((ScareScrowType)sequence);
        curSequence = sequence;
        canSpeak = true;
        boundColliders[curSequence - 1].isTrigger = false;  // 이전 허수아비에게 돌아가지 못합니다.
    }

    void SetScarescrow(ScareScrowType type)
    {
        UIManager.instance.curUIGroup.childUI[0].SetActive(true);

        curScarescrow = scareScrows[(int)type];
        curBoundCollider = boundColliders[(int)type];
    }

    void TutorialUIforAnim(string animName, bool state, TutorialUIListOrder attachUIIdx)
    {
        UIManager.instance.curUIGroup.SwitchAnim(animName, state);
        UIManager.instance.curUIGroup.AttachUIforPlayer((int)attachUIIdx);
    }

    void ManageEvent()
    {
        switch (curSequence)
        {
            case 1:
                MonsterTutorial monster = monsters.transform.GetChild(0).GetComponent<MonsterTutorial>();
                if (curIdx == 1)
                {
                    monster.gameObject.SetActive(true);
                    TutorialUIforAnim("isAttack", true, TutorialUIListOrder.BasicSkill);
                    onTutorials = CheckAttack;
                }
                else if (curIdx == 2)
                {
                    TutorialUIforAnim("isDash", true, TutorialUIListOrder.BasicSkill);
                    onTutorials = CheckDash;
                }
                else if (curIdx == 3)
                {
                    killState = 2;
                    monster.ChangeChaseState(true);
                    onTutorials = CheckKill;
                }
                else if (curIdx == 4)
                {
                    generateBullet = true;
                    //curBoundCollider.isTrigger = true;
                    isInteraction = false;
                }
                break;

            case 2:
                if (curIdx == 0) {
                    TutorialUIforAnim("isReload", true, TutorialUIListOrder.Reload);
                    onTutorials = CheckReload;
                }
                break;
            case 3:
                if (curIdx == 1)
                {
                    monsters.transform.GetChild(0).gameObject.SetActive(true);
                    monsters.transform.GetChild(1).gameObject.SetActive(true);
                    monsters.transform.GetChild(2).gameObject.SetActive(true);
                    boundColliders[6].isTrigger = true;

                    onTutorials = CheckBattle;
                }
                break;

            case 4:
                if (curIdx == 4)
                {
                    jokbo = Instantiate(jokbo,
                        GameManager.instance.player.transform.position + Vector3.right,
                        Quaternion.identity, transform);
                    onTutorials = CheckGetJokbo;
                }
                else if (curIdx == 5) {
                    TutorialUIforAnim("isOpenJokbo", true, TutorialUIListOrder.OpenJokbo);
                    onTutorials = CheckOpenJokbo;
                }
                break;

            case 5:
                if (curIdx == 1)
                {   // skill - JunButterfly
                    blanket = Instantiate(blanket,
                        GameManager.instance.player.transform.position + Vector3.right,
                        Quaternion.identity, transform);
                    onTutorials = CheckGiveSkill;
                }
                else if (curIdx == 2) {
                    TutorialUIforAnim("isHwatuSkill", true, TutorialUIListOrder.HwatuSkill);
                    onTutorials = CheckSkill;
                } 
                else if (curIdx == 3)
                {
                    TutorialUIforAnim("isTab", true, TutorialUIListOrder.HwatuSkill);
                    onTutorials = CheckTab;
                }
                else if (curIdx == 4)
                {
                    onTutorials = CheckKnockBack;
                    monsters.transform.GetChild(0).gameObject.SetActive(true);  // 넉백 당할 참새 소환
                }
                break;
        }
    }

    IEnumerator ManageDialog()
    {   // 첫번째 이벤트는 씬에 입장하자마자 1.5초 뒤에 자동으로 진행됩니다
        yield return null;

        if(!isInteraction)
        {
            SetNextDialog();    // curIdx = 0
            curIdx++;
            yield return new WaitForSeconds(.5f);
            TutorialUIforAnim("isInteraction", true, TutorialUIListOrder.Interation);
            yield return new WaitForSeconds(1f);

            SetNextDialog();    // curIdx = 1
            curIdx++;
        }

        yield return new WaitUntil(() => isInteraction);
        UIManager.instance.curUIGroup.SwitchAnim("isInteraction", false);

        canSpeak = true;
        isActiveDone = true;
    }

    void SetNextDialog()
    {
        if(tutorialDatas[curSequence].dialogues.Count > curIdx)
        {
            dialogTxt.text = tutorialDatas[curSequence].dialogues[curIdx].dialog;
            if (tutorialDatas[curSequence].dialogues[curIdx].isAction)
            {   // 현재 대화에 대하여 발생될 이벤트가 있다면 대화 진행을 멈추고 이벤트에 맞는 함수를 지정합니다.
                isInteraction = true;
                canSpeak = false;

                playerInteraction.ChangePlayerInteractionState(false);
                ManageEvent();
            }
        }
    }

    void IsDone()
    {
        UIManager.instance.curUIGroup.childUI[0].SetActive(false);  // 말풍선 종료
        checkSequenceDone[curSequence] = true;
        dialogTxt.text = "";

        // -------------- 다음을 위한 초기화
        curSequence = -1;   
        curIdx = 0;
        canSpeak = false;
        isInteraction = false;
        isActiveDone = false;

        curScarescrow = null;
        curBoundCollider.isTrigger = true;  // 다음 이동을 위한 콜리전 해제

        GameManager.instance.player
            .GetComponentInChildren<PlayerInteraction>()
            .ChangePlayerInteractionState(false);    // 상호작용 종료
    }

    #region CheckTutorialSituation
    public static int killState = 0;
    bool CheckAttack()
    {
        if (killState == 1)
        {
            UIManager.instance.curUIGroup.SwitchAnim("isAttack", false);
            isInteraction = false;
            return true;
        }
        else return false;
    }

    bool CheckDash()
    {
        if (GameManager.instance.player.IsDash())
        {
            UIManager.instance.curUIGroup.SwitchAnim("isDash", false);
            isInteraction = false;
            return true;
        }
        else return false;
    }

    bool CheckKill()
    {
        if (killState == 3)
        {
            killState = 4;
            isInteraction = false;
            return true;
        }
        else return false;
    }

    bool CheckReload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UIManager.instance.curUIGroup.SwitchAnim("isReload", false);
            isInteraction = false;
            return true;
        }
        return false;
    }

    public static int deadCnt = 0;
    bool CheckBattle()
    {
        if (deadCnt >= 3)
        {
            isInteraction = false;
            return true;
        }
        return false;
    }

    public static bool getJokbo = false;
    bool CheckGetJokbo()
    {
        if (getJokbo)
        {
            jokbo.SetActive(false);
            isInteraction = false;
            return true;
        }
        return false;
    }

    public static bool closeJokbo = false;
    bool CheckOpenJokbo()
    {   // 한번 열었다가 닫아야 다음 대화 진행
        if (!closeJokbo && Input.GetKeyDown(KeyCode.K))
        {
            UIManager.instance.curUIGroup.SwitchAnim("isOpenJokbo", false);
        }
        if (closeJokbo)
        {
            isInteraction = false;
            return true;
        }
        return false;
    }

    public static bool isBlankBulletCard = false;
    bool CheckGiveSkill()
    {
        if (isBlankBulletCard)
        {
            blanket.SetActive(false);
            isInteraction = false;
            return true;
        }
        return false;
    }

    bool CheckSkill()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UIManager.instance.curUIGroup.SwitchAnim("isHwatuSkill", false);
            isInteraction = false;
            return true;
        }
        return false;
    }

    public static bool isTab = false;
    bool CheckTab()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UIManager.instance.curUIGroup.SwitchAnim("isTab", false);
            isTab = true;
            isInteraction = false;
            return true;
        }
        return false;
    }

    public static bool useSkill = false;
    public static bool isWarriorDied = false;
    bool CheckKnockBack()
    {
        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E)) && monsters.transform.GetChild(0).GetComponent<MonsterTutorial>().isKnockedBack)
        {
            useSkill = true;
        }
        if(useSkill && isWarriorDied)
        {
            isInteraction = false;
            return true;
        }
        return false;
    }

    public static void FindBlankBullet()
    {   // 튜토리얼 스킬 지급 (공포탄)
        for (int i = 0; i < SkillManager.instance.hwatuData.Length; i++)
        {
            if (SkillManager.instance.hwatuData[i].hwatu.type == SeotdaHwatuName.JunButterfly)
            {
                //HwatuData blankBullet = SkillManager.instance.hwatuData[i];
                //SkillManager.instance.AddSkill(blankBullet);
                //break;
            }
        }
    }
    #endregion
}
