using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

public class TutorialInteraction : Interaction
{
    [SerializeField] List<GameObject> scareScrows;
    [SerializeField] List<BoxCollider2D> boundColliders;

    public enum TutorialMonsters
    {
        attack, battle1, battle2, battle3, hwatu12, hwatu13, skill
    }
    [System.Serializable]
    public class MonsterState
    {
        public GameObject monster;
        public bool isKilled;
    }
    public List<MonsterState> monsters;

    [SerializeField] GameObject jokbo;
    [SerializeField] BlanketInteractionData blanket;
    BlanketInteraction blanketInteraction;
    TutorialDBEntity[] tutoDB;

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
    public Transform[] childrens;
    TextMeshProUGUI[] dialogTxts;   // 0 : 허수아비 머리 위, 1 : npc 대화처럼

    [Header("Dialog State")]
    public bool canSpeak = false;
    public bool isInteraction = false;
    public bool isActiveDone = false;
    public bool isNPCImg = false; // 족보와 모포 활성화시 true가 되고 말풍선이 아니라 전체 화면 안내로 전환

    public int curSequence;
    public int curIdx = 0;
    public bool[] checkSequenceDone;

    public static bool generateBullet = false;

    private async void Start()
    {
        await LoadTutorialDBEntity();
        checkSequenceDone = new bool[tutorialDatas.Count];
        childrens = UIManager.instance.curUIGroup.childUI[0].transform.GetComponentsInChildren<Transform>(true);
        dialogTxts = UIManager.instance.curUIGroup.childUI[0].GetComponentsInChildren<TextMeshProUGUI>(true);

        jokboUIGroup = UIManager.instance.SceneUI["Jokbo"].GetComponent<JokboUIGroup>();
        StartFirstDialog();
    }

    private void Update()
    {
        if(curScarescrow != null)
        {   // 허수아비의 머리 위에 말풍선을 위치
            childrens[1].position = 
                Camera.main.WorldToScreenPoint(curScarescrow.transform.position) + padding;
        }
        if (curSequence < 0) return;

        if ((canSpeak && Input.GetKeyDown(KeyCode.F)) || isActiveDone)
        {   // 일반 대화 출력
            if (isActiveDone) { isActiveDone = false; canSpeak = true; }

            SetNextDialog();
            
            if (curIdx >= tutorialDatas[curSequence].dialogues.Count)
                IsDone();
            else if (curIdx == tutorialDatas[curSequence].dialogues.Count - 1)
                ClearCurStage();
            else curIdx++;
        }
        else if (curSequence > 0 && isInteraction)
        {   // 현재 대화에 대한 이벤트 호출
            if (OccurTutorial(onTutorials))
            {
                isInteraction = false;
                isActiveDone = true;
                UIManager.instance.curUIGroup.AttachUIforPlayer(-1);
            }
        }
    }

    private void OnDestroy()
    {   // 튜토리얼 종료
        SkillManager.instance.DeleteSkill(SeotdaHwatuCombination.TT3);
        Player.instance.curHP = Player.instance.maxHP;
    }

    public GameObject GetCurScarescrow() => curScarescrow;

    async Task LoadTutorialDBEntity()
    {
        tutoDB = await DataManager.instance.GetValues<TutorialDBEntity>(SheetType.Tutorial, "A1:D");

        int curSequence = -1;
        for (int i = 0; i < tutoDB.Length; i++)
        {
            curSequence++;
            List<DialogAction> tmpDialogs = new List<DialogAction>();
            while (curSequence == tutoDB[i].sequence)
            {
                tmpDialogs.Add(new DialogAction(tutoDB[i].dialogue, tutoDB[i].isInteraction));
                if (tutoDB.Length == i + 1 || curSequence != tutoDB[i + 1].sequence)
                {
                    tutorialDatas.Add(new TutorialData(curSequence, tmpDialogs));
                    break;
                }
                else i++;
            }
        }
        ScenesManager.instance.isLoadedDB++;
    }

    void StartFirstDialog()
    {
        curSequence = 0;
        SetScarescrow(ScareScrowType.Start);
        
        playerInteraction = Player.instance.GetComponentInChildren<PlayerInteraction>();
        StartCoroutine(StartDialog());
    }

    public override void LoadEvent(InteractionData data)
    {   // player가 상호작용한 허수아비에 맞춰 이벤트가 진행됩니다.
        SetScarescrow((ScareScrowType)data.sequence);
        StartCurStage(data.sequence);
        jokboUIGroup.isPossibleJokbo = false;   // 족보 대화 중ㅇㅔ는 활성화 불가
    }

    void SetScarescrow(ScareScrowType type)
    {
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
            case 1: // 공격 & 대시
                if (curIdx == 1)
                { 
                    Player.instance.isCombatZone = true;
                    onTutorials = CheckAttack;
                    monsters[(int)TutorialMonsters.attack].monster.SetActive(true);

                    TutorialUIforAnim("isAttack", true, TutorialUIListOrder.BasicSkill);
                }
                else if (curIdx == 2)
                {
                    TutorialUIforAnim("isDash", true, TutorialUIListOrder.BasicSkill);
                    onTutorials = CheckDash;
                }
                else if (curIdx == 3)
                {
                    monsters[(int)TutorialMonsters.attack].monster.GetComponent<TutorialFar>().ChaseState(true);

                    onTutorials = CheckKill;
                }
                else if (curIdx == 4)
                {
                    generateBullet = true;
                }
                break;
            case 2: // 재장전
                if (curIdx == 0) {
                    TutorialUIforAnim("isReload", true, TutorialUIListOrder.Reload);
                    onTutorials = CheckReload;
                }
                break;
            case 3: // 모의 전투
                if (curIdx == 1)
                {
                    monsters[(int)TutorialMonsters.battle1].monster.SetActive(true);
                    monsters[(int)TutorialMonsters.battle2].monster.SetActive(true);
                    monsters[(int)TutorialMonsters.battle3].monster.SetActive(true);
                    boundColliders[6].isTrigger = true;

                    onTutorials = CheckBattle;
                }
                break;
            case 4: // 족보
                if(curIdx == 6)
                {
                    jokbo = Instantiate(jokbo,
                        Player.instance.transform.position + Vector3.right,
                        Quaternion.identity, transform);
                    onTutorials = CheckGetJokbo;
                }
                else if (curIdx == 7) {
                    jokboUIGroup.isPossibleJokbo = true;
                    TutorialUIforAnim("isOpenJokbo", true, TutorialUIListOrder.OpenJokbo);
                    onTutorials = CheckOpenJokbo;
                }
                else if(curIdx == 9)
                {
                    isNPCImg = false;
                }
                else if(curIdx == 10)
                {
                    jokboUIGroup.JokboState(false);
                }
                break;
            case 5: // skill
                if (curIdx == 5)
                {
                    onTutorials = CheckGetHwatu;
                }
                else if (curIdx == 6)
                {
                    onTutorials = CheckBlanket;
                    isNPCImg = true;
                }
                else if (curIdx == 7)
                {   // 아래에 있는 화투패를 드래그 해서 모포 2장 올려놓으면 스킬을 만들 수 있어!
                    blanketInteraction = playerInteraction.blanketInteraction as BlanketInteraction;
                    Player.instance.isCombatZone = false;
                    onTutorials = CheckSkillinBlanket;
                }
                else if (curIdx == 11)
                {
                    isNPCImg = false;
                    isBlanket = true;

                    Player.instance.isCombatZone = true;
                    onTutorials = CheckUseSkill;
                }
                break;
        }
    }

    public void OnBattleMonster()
    {
        monsters[(int)TutorialMonsters.battle1].monster.GetComponent<TutorialFar>().ChaseState(true);
        monsters[(int)TutorialMonsters.battle2].monster.GetComponent<TutorialFar>().ChaseState(true);
        monsters[(int)TutorialMonsters.battle3].monster.GetComponent<TutorialFar>().ChaseState(true);
    }

    IEnumerator StartDialog()
    {
        UIManager.instance.SceneUI["Battle_1"].GetComponent<UIGroup>().childUI[3].SetActive(false);
        yield return new WaitUntil(()=> UIManager.instance.isEndFade);

        if (!isInteraction)
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
        int curTxt = 0;
        if (isNPCImg)
        {
            childrens[1].gameObject.SetActive(false);
            childrens[3].gameObject.SetActive(true);
            curTxt = 1;
        }
        else
        {
            childrens[3].gameObject.SetActive(false);
            childrens[1].gameObject.SetActive(true);
        }

        if (tutorialDatas[curSequence].dialogues.Count > curIdx)
        {
            dialogTxts[curTxt].text = tutorialDatas[curSequence].dialogues[curIdx].dialog;
            if (tutorialDatas[curSequence].dialogues[curIdx].isAction)
            {   // 현재 대화에 대하여 발생될 이벤트가 있다면 대화 진행을 멈추고 이벤트에 맞는 함수를 지정합니다.
                isInteraction = true;
                canSpeak = false;

                Player.instance.ChangePlayerInteractionState(false);
            }
            ManageEvent();
        }
    }

    public void IsDone()
    {
        Debug.Log("Isdone");
        curScarescrow = null;

        childrens[1].gameObject.SetActive(false);
        childrens[3].gameObject.SetActive(false);

        dialogTxts[0].text = "";
        dialogTxts[1].text = "";
    }

    void StartCurStage(int sequence)
    {
        curSequence = sequence;

        boundColliders[curSequence - 1].isTrigger = false;  // 이전 허수아비에게 돌아가지 못합니다.
        canSpeak = true;
    }

    void ClearCurStage()
    {
        checkSequenceDone[curSequence] = true;
        curBoundCollider.isTrigger = true;  // 다음 이동을 위한 콜리전 해제

        if (curSequence > 3) jokboUIGroup.isPossibleJokbo = true;    // 족보 이용 가능

        isInteraction = false;
        curSequence = -1;
        curIdx = 0;
        canSpeak = false;
        isActiveDone = false;

        Player.instance.ChangePlayerInteractionState(false);    // 상호작용 종료
    }

    #region CheckTutorialSituation
    public bool isAttacked = false;
    bool CheckAttack()
    {
        if (isAttacked)
        {
            UIManager.instance.curUIGroup.SwitchAnim("isAttack", false);
            return true;
        }
        else return false;
    }

    public bool isDashed = false;
    bool CheckDash()
    {
        if (Player.instance.IsDash())
        {
            isDashed = true;
            UIManager.instance.curUIGroup.SwitchAnim("isDash", false);
            return true;
        }
        else return false;
    }

    bool CheckKill()
    {
        if (monsters[(int)TutorialMonsters.attack].isKilled)
            return true;
        else return false;
    }

    bool CheckReload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UIManager.instance.curUIGroup.SwitchAnim("isReload", false);
            return true;
        }
        return false;
    }

    bool CheckBattle()
    {
        if (monsters[(int)TutorialMonsters.battle1].isKilled &
            monsters[(int)TutorialMonsters.battle2].isKilled &
            monsters[(int)TutorialMonsters.battle3].isKilled)
        {
            return true;
        }
        return false;
    }

    public JokboUIGroup jokboUIGroup;
    bool CheckGetJokbo()
    {
        if (jokboUIGroup.isPossibleJokbo)
        {   // 상호작용을 통해 족보 획득
            jokbo.SetActive(false);
            return true;
        }
        return false;
    }

    bool CheckOpenJokbo()
    {   
        if (Input.GetKeyDown(KeyCode.K))
        {
            UIManager.instance.curUIGroup.SwitchAnim("isOpenJokbo", false);
            UIManager.instance.SceneUI["Battle_1"].SetActive(false);
            UIManager.instance.SceneUI["Inventory"].SetActive(false);
            isNPCImg = true;
            jokboUIGroup.isPossibleJokbo = false; // 족보 관련 대화 종료까지는 족보 닫기 불가
            return true;
        }
        else return false;
    }

    bool isHwatuMonster = true;
    bool CheckGetHwatu()
    {
        if (isHwatuMonster && Input.GetKeyDown(KeyCode.F))
        {
            UIManager.instance.SceneUI["Battle_1"].SetActive(true);
            UIManager.instance.SceneUI["Inventory"].SetActive(true);

            monsters[(int)TutorialMonsters.hwatu12].monster.SetActive(true);
            monsters[(int)TutorialMonsters.hwatu13].monster.SetActive(true);
            isHwatuMonster = false;
        }
        if (SkillManager.instance.materialCardCnt >= 2) return true;
        else return false;
    }

    public static bool isBlanketInteration = false;
    bool CheckBlanket()
    {
        blanket.isClear = true;
        blanket.sequence = 1;

        if (isBlanketInteration)
        {
            UIManager.instance.SceneUI["Battle_1"].GetComponent<UIGroup>().childUI[3].SetActive(true);
            return true;
        }
        else return false;
    }

    public static bool isBlanket = false;
    bool CheckSkillinBlanket()
    {
        if (blanketInteraction.selectedCnt == 2)
        { // 화투패 2장 획득 완료
            return true;
        }
        else return false;
    }

    public void OnTrashHwatu()
    {
        dialogTxts[1].text = "튜토리얼 중에는 화투패를 버릴 수 없어.";
    }
    public void OnTrashSkill()
    {
        dialogTxts[1].text = "튜토리얼 중에는 스킬을 버릴 수 없어.";
    }

    public bool useSkill = false;
    bool isLoadSkillMonster = false;
    bool CheckUseSkill()
    {   // 3땡 : 일정 시간동안 브레스영역이 활성화 되고 여기 닿으면 데미지
        
        if(!isLoadSkillMonster && !UIManager.instance.SceneUI["Battle_1"].GetComponent<UIGroup>().childUI[5].activeSelf)
        {   // 모포가 꺼지면 몬스터 소환
            childrens[0].gameObject.SetActive(false);
            isLoadSkillMonster = true;
            monsters[(int)TutorialMonsters.skill].monster.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
            useSkill = true;

        if(useSkill && monsters[6].isKilled)
        {
            childrens[0].gameObject.SetActive(true);
            return true;
        }
        return false;
    }
    #endregion
}
