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

public class Tutorial : MonoBehaviour
{
    [SerializeField] List<GameObject> scareScrows;
    [SerializeField] List<BoxCollider2D> boundColliders;
    [SerializeField] TutorialDB tutoDB;
    [SerializeField] GameObject monsters;
    [SerializeField] GameObject jokbo;

    public Vector3 padding;
    public GameObject curScarescrow = null;
    public BoxCollider2D curBoundCollider = null;
    PlayerInteraction playerInteraction;

    delegate bool OnTutorials();
    OnTutorials onTutorials;

    bool OccurTutorial(OnTutorials tutoFunction)
    {
        return tutoFunction();
    }

    // DialogInteraction Info
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

    // DialogUI
    TextMeshProUGUI dialogTxt;

    // CheckVariable
    public bool canSpeak = false;
    public bool isInteraction = false;
    public bool isActiveDone = false;
    public int curSequence;
    public int curIdx = 0;

    private void Awake()
    {
        LoadDialog();
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
            UIManager.instance.curUIGroup.transform.position = 
                Camera.main.WorldToScreenPoint(curScarescrow.transform.position) + padding;
        }

        if ((canSpeak && Input.GetKeyDown(KeyCode.F)) || isActiveDone)
        {   // 일반 대화 출력
            if (isActiveDone) isActiveDone = false;

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
            }
        }
    }

    void LoadDialog()
    {   // 튜토리얼 대화를 시퀀스별로 불러와 저장합니다.
        int curSequence = -1;
        for (int i = 0; i < tutoDB.TutorialEntity.Count; i++)
        {
            curSequence++;
            List<DialogAction> tmpDialogs = new List<DialogAction>();
            while (curSequence == tutoDB.TutorialEntity[i].sequence)
            {
                tmpDialogs.Add(new DialogAction(tutoDB.TutorialEntity[i].dialogue, tutoDB.TutorialEntity[i].isInteraction));
                if (tutoDB.TutorialEntity.Count == i+1 || curSequence != tutoDB.TutorialEntity[i + 1].sequence)
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
    }

    void SetScarescrow(ScareScrowType type)
    {
        UIManager.instance.curUIGroup.childUI[0].SetActive(true);

        curScarescrow = scareScrows[(int)type];
        curBoundCollider = boundColliders[(int)type];
    }

    void ManageEvent()
    {
        switch (curSequence)
        {
            case 1:
                if (curIdx == 1)
                {
                    // 매를 소환하기 -> 이동 x, 공격 x
                    monsters.transform.GetChild(0).gameObject.SetActive(true);

                    onTutorials = CheckAttack;
                }
                else if (curIdx == 2) onTutorials = CheckDash;
                else if (curIdx == 3)
                {
                    GetComponentInChildren<BulletGenerator>(true).gameObject.SetActive(true);
                    isInteraction = false;
                }
                break;

            case 2:
                if (curIdx == 0) onTutorials = CheckReload;
                break;

            case 4:
                if (curIdx == 4)
                {
                    jokbo = Instantiate(jokbo,
                        GameManager.instance.player.transform.position + Vector3.right,
                        Quaternion.identity, transform);
                    onTutorials = CheckGetJokbo;
                }
                else if (curIdx == 5) onTutorials = CheckOpenJokbo;
                break;

            case 5:
                if (curIdx == 2) onTutorials = CheckGiveSkill;
                if (curIdx == 3) onTutorials = CheckSkill;
                if (curIdx == 4) onTutorials = CheckTab;
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
            yield return new WaitForSeconds(1.5f);

            SetNextDialog();    // curIdx = 1
            curIdx++;
        }

        yield return new WaitUntil(() => isInteraction);
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

        // -------------- 다음을 위한 초기화
        curSequence = -1;   
        curIdx = 0;
        canSpeak = false;
        isInteraction = false;
        isActiveDone = false;

        curScarescrow = null;
        curBoundCollider.isTrigger = true;  // 다음 이동을 위한 콜리전 해제
        
        GameManager.instance.player.GetComponentInChildren<PlayerInteraction>().dialogueInteraction.isDone = true;
    }

    // ---------- check tutorial situation
    bool CheckAttack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isInteraction = false;
            return true;
        }
        else return false;
    }

    bool CheckDash()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && GameManager.instance.player.IsDash())
        {
            isInteraction = false;
            return true;
        }
        else return false;
    }

    bool CheckReload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            isInteraction = false;
            return true;
        }
        return false;
    }

    public bool getJokbo = false;
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

    bool CheckOpenJokbo()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            isInteraction = false;
            return true;
        }
        return false;
    }

    bool CheckGiveSkill()
    {
        return false;

    }

    bool CheckSkill()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isInteraction = false;
            return true;
        }
        return false;

    }

    bool CheckTab()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isInteraction = false;
            return true;
        }
        return false;

    }
}
