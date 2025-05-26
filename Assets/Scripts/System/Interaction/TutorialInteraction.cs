using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public enum ScareScrowType
{
    Start, AttackDash, Bullet, Reload, MiniBattle, Jokbo, Skill, None
}

public enum TutorialMonsters
{
    attack, battle1, battle2, hwatu12, hwatu13, skill
}

public enum DialogType
{
    Bubble, Center, Right
}

public class TutorialInteraction : Interaction
{
    [System.Serializable] public class ScarescrowState
    {
        public GameObject scareScrow;
        public DialogType type;
        public bool isSequenceDone;
    }
    [Header("NPC state")]
    [SerializeField] List<ScarescrowState> scareScrows;
    public ScarescrowState curScarescrowState;
    public ScareScrowType curScarescrowType;

    [SerializeField] List<GameObject> doors;
    Animator doorAnim;
    
    [System.Serializable]  public class MonsterState
    {
        public GameObject monster;
        public bool isKilled;
    }
    [Header("Spawn Objs")]
    public List<MonsterState> monsters;

    [SerializeField] GameObject jokbo;
    [SerializeField] BlanketInteractionData blanket;
    MaterialHwatuSlotUI[] materialHwatuSlotUIs;
    BlanketInteraction blanketInteraction;
    PlayerInteraction playerInteraction;
    TutorialDBEntity[] tutoDB;

    [Header("BulletDashTutorial")]
    public GameObject bulletGenerator;
    public GameObject bulletEndAction;
    public static bool generateBullet = false;
    
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

    [Header("DialogUI")]
    public TutorialUIGroup tutorialUIGroup;
    public JokboUIGroup jokboUIGroup;
    public GameObject curDialogUI;
    public TextMeshProUGUI curDialogTxt;

    [Header("Dialog State")]
    public bool canSpeak = false;
    public bool isInteraction = false;
    public bool interactionF = false;
    public bool isActiveDone = false;
    public bool isLastDance = false;

    public int curIdx = 0;
    public float paddingValue = 195f;

    delegate bool OnTutorials();
    OnTutorials onTutorials;

    bool OccurTutorial(OnTutorials tutoFunction)
    {
        return tutoFunction();
    }

    async void Start()
    {
        await LoadTutorialDBEntity();
        jokboUIGroup = UIManager.instance.SceneUI["Jokbo"].GetComponent<JokboUIGroup>();
        StartFirstDialog();
    }

    void Update()
    {
        if (curScarescrowType != ScareScrowType.None && curScarescrowState.type == DialogType.Bubble)
        {   // 허수아비의 머리 위에 말풍선을 위치
            tutorialUIGroup.dialogUIs[(int)DialogType.Bubble].transform.position =
                Camera.main.WorldToScreenPoint(curScarescrowState.scareScrow.transform.position) + (Vector3.up * paddingValue);
        }
        if (curScarescrowType == ScareScrowType.None) return;

        int curSequenceIdx = GetCurSequenceIdx();

        if (interactionF && OccurTutorial(onTutorials))
        {   // 퀘스트 행동 여부 판단
            interactionF = false;
            isInteraction = false;
            isActiveDone = true;
            if (isLastDance)
            {
                isLastDance = false;
                ClearCurStage();
            }
        }
        else if (curSequenceIdx > 0 && !interactionF && isInteraction)
        {
            if (curScarescrowType == ScareScrowType.Skill && curIdx > 4)  // 여기서부터는 상호작용 키를 안눌러도 퀘스트 행동 가능
                interactionF = true;
            else if (Input.GetKeyDown(KeyCode.F))
            {   // 현재 대화에 대한 이벤트 호출 - 상호 작용 키를 누르고 퀘스트 행동 가능
                interactionF = true;
                curDialogUI.SetActive(false);
                curScarescrowState.scareScrow.SetActive(false);
                Player.instance.ChangePlayerInteractionState(false);
            }
        }
        else if ((canSpeak && Input.GetKeyDown(KeyCode.F)) || isActiveDone)
        {   // 일반 대화 출력
            if (isActiveDone) { isActiveDone = false; canSpeak = true; curScarescrowState.scareScrow.SetActive(true); }

            SetNextDialog();
            if (curIdx == tutorialDatas[curSequenceIdx].dialogues.Count - 1)
            {  // 마지막 대화이면서 해야할 일 X
                if(!tutorialDatas[curSequenceIdx].dialogues[curIdx].isAction)
                    ClearCurStage();
                else isLastDance = true;
            }
            else
                curIdx++;
        }
    }

    void OnDestroy()
    {   // 튜토리얼 종료
        SkillManager.instance.DeleteSkill(SeotdaHwatuCombination.TT3);
        Player.instance.refCurHp = Player.instance.GetMaxHP();
        Player.instance.isClearTutorial = true;
        ItemManager.instance.gunController.GetCurGunComponent().ResetBulletCnt();

        //UIManager.instance.SceneUI["Battle_1"].GetComponent<UIGroup>().childUI[1].transform.parent.gameObject.SetActive(true); //?
    }

    public GameObject GetCurScarescrowObj() => curScarescrowState.scareScrow;
    public int GetCurSequenceIdx() => (int)curScarescrowType;

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
        SetScarescrow(0);
        
        playerInteraction = Player.instance.GetComponentInChildren<PlayerInteraction>();
        StartCoroutine(StartDialog());
    }

    public override void LoadEvent(InteractionData data)
    {   // player가 상호작용한 허수아비에 맞춰 이벤트가 진행됩니다.
        SetScarescrow(data.sequence);
        StartCurStage();
    }

    void SetScarescrow(int type)
    {
        curScarescrowState = scareScrows[type];
        curScarescrowType = (ScareScrowType)type;
    }

    void SetCurDialogUI()
    {
        int curState = (int)curScarescrowState.type;

        curDialogUI = tutorialUIGroup.dialogUIs[curState];
        curDialogUI.SetActive(true);

        if(curScarescrowState.type == DialogType.Center)
            tutorialUIGroup.SetNextKey("isCenter");
        else if(curScarescrowState.type == DialogType.Right)
            tutorialUIGroup.SetNextKey("isRight");

        curDialogTxt = tutorialUIGroup.dialogTxts[curState];
    }

    void StartCurStage()
    {
        canSpeak = true;
        SetDoorState("isClose");
    }

    void ManageEvent()
    {
        switch (curScarescrowType)
        {
            case ScareScrowType.AttackDash: // 공격 & 구르기
                if (curIdx == 1)
                { 
                    Player.instance.isCombatZone = true;
                    onTutorials = CheckAttack;
                    monsters[(int)TutorialMonsters.attack].monster.SetActive(true);
                }
                else if (curIdx == 2)
                    onTutorials = CheckDash;
                else if (curIdx == 3)
                    onTutorials = CheckKill;
                break;
            case ScareScrowType.Bullet: // 총알 구르기 연습
                if (curIdx == 0)
                {
                    generateBullet = true;
                    bulletGenerator.SetActive(true);
                    bulletEndAction.SetActive(true);
                }
                break;
            case ScareScrowType.Reload: // 재장전
                if (curIdx == 1)
                    onTutorials = CheckReload;
                break;
            case ScareScrowType.MiniBattle: // 모의 전투
                if (curIdx == 1)
                {
                    monsters[(int)TutorialMonsters.battle1].monster.SetActive(true);
                    monsters[(int)TutorialMonsters.battle2].monster.SetActive(true);

                    onTutorials = CheckBattle;
                }
                break;
            case ScareScrowType.Jokbo: // 족보
                if(curIdx == 4)
                {
                    jokbo = Instantiate(jokbo,
                        GetCurScarescrowObj().transform.position + Vector3.right,
                        Quaternion.identity, transform);
                    onTutorials = CheckGetJokbo;
                }
                else if (curIdx == 5) {
                    curScarescrowState.type = DialogType.Right;
                    onTutorials = CheckOpenJokbo;
                }
                else if(curIdx == 7)
                    curScarescrowState.type = DialogType.Bubble;
                else if (curIdx == 8)
                {
                    jokboUIGroup.isPossibleJokbo = true;
                    jokboUIGroup.JokboState(false);
                }
                break;
            case ScareScrowType.Skill: // skill
                if(curIdx == 0)
                    jokboUIGroup.isPossibleJokbo = false;
                if (curIdx == 2)
                    onTutorials = CheckGetHwatu;
                else if (curIdx == 3)
                {
                    onTutorials = CheckBlanket;
                    curScarescrowState.type = DialogType.Right;
                }
                else if(curIdx == 4)
                {
                    Player.instance.ChangePlayerInteractionState(true);

                    tutorialUIGroup.SwitchAnim("isSkillInfo", true);
                    materialHwatuSlotUIs =   UIManager.instance.presenters[(int)PresenterType.Skill].objs[0].GetComponentsInChildren<MaterialHwatuSlotUI>(); //모포에 생성된 화투패들 소집
                    foreach (MaterialHwatuSlotUI material in materialHwatuSlotUIs)
                    {
                        material.isSelected = true;
                    }
                }
                else if (curIdx == 5)
                {   // 아래에 있는 화투패를 드래그 해서 모포 2장 올려놓으면 스킬을 만들 수 있어
                    tutorialUIGroup.SwitchAnim("isSkillInfo", false);
                    tutorialUIGroup.SwitchAnim("isHwatuCombination", true);

                    blanketInteraction = playerInteraction.blanketInteraction as BlanketInteraction;
                    Player.instance.isCombatZone = false;

                    foreach (MaterialHwatuSlotUI material in materialHwatuSlotUIs)
                    {
                        material.isSelected = false;
                    }
                    onTutorials = CheckSkillinBlanket;
                }
                else if(curIdx == 7)
                    tutorialUIGroup.SwitchAnim("isSkillKey", true);
                else if(curIdx == 8)
                    tutorialUIGroup.SwitchAnim("isSkillKey", false);
                else if(curIdx == 9)
                    tutorialUIGroup.SwitchAnim("isTrash", true);
                else if (curIdx == 10)
                {
                    tutorialUIGroup.SwitchAnim("isTrash", false);

                    Player.instance.isCombatZone = true;

                    onTutorials = CheckUseSkill;
                    curScarescrowState.type = DialogType.Bubble;
                }
                break;
        }
    }

    IEnumerator StartDialog()
    {
        yield return new WaitUntil(()=> UIManager.instance.isEndFade);

        if (!isInteraction)
        {
            SetNextDialog();    // curIdx = 0
            curIdx++;
            yield return new WaitForSeconds(.5f);

            //tutorialUIGroup.SwitchAnim("isInteraction", true);
            yield return new WaitForSeconds(1f);

            SetNextDialog();    // curIdx = 1
            curIdx++;
        }

        yield return new WaitUntil(() => isInteraction);
        //tutorialUIGroup.SwitchAnim("isInteraction", false);

        canSpeak = true;
        isActiveDone = true;
    }

    void SetNextDialog()
    {
        tutorialUIGroup.InactiveAllDialogUIs();
        SetCurDialogUI();

        int curSequenceIdx = GetCurSequenceIdx();
        if (tutorialDatas[curSequenceIdx].dialogues.Count > curIdx)
        {
            DialogAction curDialog = tutorialDatas[curSequenceIdx].dialogues[curIdx];
            curDialogTxt.text = curDialog.dialog;

            if (curDialog.isAction)
            {   // 현재 대화에 대하여 발생될 이벤트가 있다면 대화 진행을 멈추고 이벤트에 맞는 함수를 지정합니다.
                isInteraction = true;
                canSpeak = false;
            }
            ManageEvent();
        }
    }

    void ClearCurStage()
    {
        curScarescrowState.isSequenceDone = true;

        int curSequenceIdx = GetCurSequenceIdx();
        doorAnim = doors[curSequenceIdx].GetComponent<Animator>();
        if(curScarescrowType != ScareScrowType.Bullet)
            SetDoorState("isOpen");

        if (curSequenceIdx > 4) jokboUIGroup.isPossibleJokbo = true;    // 족보 이용 가능

        canSpeak = false;
        isInteraction = false;
        isActiveDone = false;
        curIdx = 0;

        Player.instance.ChangePlayerInteractionState(false);    // 상호작용 종료
    }

    public void SetDoorState(string animName)
    {
        if(animName.Equals("isOpen"))
            SoundManager.instance.SetEffectSound(SoundType.UI, UISfx.doorOpen);
        else
            SoundManager.instance.SetEffectSound(SoundType.UI, UISfx.doorClose);

        doorAnim?.SetTrigger(animName);
    }

    public void IsDone()
    {
        curScarescrowType = ScareScrowType.None;

        tutorialUIGroup.ResetDialogTxts();
        tutorialUIGroup.InactiveAllDialogUIs();
    }

    #region CheckTutorialSituation
    public bool isAttacked = false;
    bool CheckAttack()
    {
        tutorialUIGroup.SwitchAnim("isAttack", true);

        if (isAttacked)
        {
            tutorialUIGroup.SwitchAnim("isAttack", false);
            return true;
        }
        else return false;
    }

    public bool isDashed = false;
    bool CheckDash()
    {
        tutorialUIGroup.SwitchAnim("isDash", true);
        monsters[(int)TutorialMonsters.attack].monster.GetComponent<TutorialBirdArcher1>().SetAttackable();

        if (Player.instance.IsDash())
        {
            isDashed = true;
            tutorialUIGroup.SwitchAnim("isDash", false);
            return true;
        }
        else return false;
    }

    bool isStartKill = true;
    bool CheckKill()
    {
        if (isStartKill)
        {
            isStartKill = false;
            monsters[(int)TutorialMonsters.attack].monster.GetComponent<TutorialBirdArcher1>().SetAttackable();
        }

        if (monsters[(int)TutorialMonsters.attack].isKilled)
            return true;
        else return false;
    }

    bool CheckReload()
    {
        tutorialUIGroup.SwitchAnim("isReload", true);

        if (Input.GetKeyDown(KeyCode.R))
        {
            tutorialUIGroup.SwitchAnim("isReload", false);
            return true;
        }
        return false;
    }

    bool isStartBattle = true;
    bool CheckBattle()
    {
        if (isStartBattle)
        {
            isStartBattle = false;
            TutorialBirdArcher1 far1 = monsters[(int)TutorialMonsters.battle1].monster.GetComponent<TutorialBirdArcher1>();
            far1.SetAttackable();

            TutorialBirdArcher1 far2 = monsters[(int)TutorialMonsters.battle2].monster.GetComponent<TutorialBirdArcher1>();
            far2.SetAttackable();
        }

        if (monsters[(int)TutorialMonsters.battle1].isKilled &
            monsters[(int)TutorialMonsters.battle2].isKilled )
            return true;
        return false;
    }

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
        tutorialUIGroup.SwitchAnim("isOpenJokbo", true);

        if (Input.GetKeyDown(KeyCode.K))
        {
            tutorialUIGroup.SwitchAnim("isOpenJokbo", false);
            jokboUIGroup.isPossibleJokbo = false; // 족보 관련 대화 종료까지는 족보 닫기 불가
            return true;
        }
        else return false;
    }

    bool isHwatuMonster = true;
    bool CheckGetHwatu()
    {
        if (isHwatuMonster)
        {
            isHwatuMonster = false;

            monsters[(int)TutorialMonsters.hwatu12].monster.SetActive(true);
            monsters[(int)TutorialMonsters.hwatu13].monster.SetActive(true);
        }
        if (ItemManager.instance.refHwatuCardCnt >= 2)
            return true;
        else return false;
    }

    public static bool isBlanketInteration = false;
    bool CheckBlanket()
    {
        blanket.isClear = true;
        blanket.sequence = 1;

        if (isBlanketInteration)
        {
            jokboUIGroup.isPossibleJokbo = true;
            return true;
        }
        else return false;
    }

    public static bool isBlanket = false;
    public static bool isPossibleCombination = false;
    bool CheckSkillinBlanket()
    {
        if (blanketInteraction.selectedCnt == 2)
        { // 화투패 2장 획득 완료
            blanket.isActive = true;
            tutorialUIGroup.SwitchAnim("isHwatuCombination", false);
            return true;
        }
        else return false;
    }

    public void OnTrashHwatu()
    {
        curDialogTxt.text = "튜토리얼 중에는 화투패를 버릴 수 없어.";
    }
    public void OnTrashSkill()
    {
        curDialogTxt.text = "튜토리얼 중에는 스킬을 버릴 수 없어.";
    }

    public void OnJokboInBlanket()
    {
        curDialogTxt.text = "스킬 조합을 모를 때를 대비해 족보를 잘 활용해봐.\n족보를 닫고 모포로 돌아가보자.";
    }

    public bool useSkill = false;
    bool isLoadSkillMonster = false;
    bool CheckUseSkill()
    {   // 3땡 : 일정 시간동안 브레스영역이 활성화 되고 여기 닿으면 데미지
        if(UIManager.instance.curOpenUI.Count == 1) isBlanket = true;
        else if(UIManager.instance.curOpenUI.Count > 1) isBlanket = false;

        if (!isLoadSkillMonster && !UIManager.instance.GetPresentersUIState(PresenterType.Skill, 0))
        {   // 모포가 꺼지면 몬스터 소환
            isLoadSkillMonster = true;
            
            jokboUIGroup.isPossibleJokbo = false;
            curDialogUI.SetActive(false);
            monsters[(int)TutorialMonsters.skill].monster.SetActive(true);
            Player.instance.ChangePlayerInteractionState(false);
        }
        else if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
            useSkill = true;

        if(useSkill && monsters[(int)TutorialMonsters.skill].isKilled)
        {
            curDialogUI.SetActive(true);
            return true;
        }
        return false;
    }
    #endregion
}
