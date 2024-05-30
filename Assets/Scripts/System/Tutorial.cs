using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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
    public Vector3 padding;
    public GameObject curScarescrow = null;
    public BoxCollider2D curBoundCollider = null;

    // DialogInfo
    List<TutorialData> tutorialDatas = new List<TutorialData>();
    struct TutorialData {
        // sequence별로 대화를 저장하는 구조체
        public int sequence;
        public List<string> dialogues;

        public TutorialData(int _sequence, List<string> _dialogues)
        {
            sequence = _sequence;
            dialogues = _dialogues;
        }
    }

    // DialogUI
    TextMeshProUGUI dialogTxt;

    // CheckVariable
    public bool isInteraction;
    public bool isAttack, isDash;
    public bool isReload;
    public bool isMiniBattle;
    public bool getJokbo, openJokbo;
    public bool isSkill, isTab;

    private void Awake()
    {
        LoadDialog();
        Invoke("StartFirstDialog", 1.5f);
    }

    void StartFirstDialog()
    {
        LoadEvent(0);
    }

    private void Update()
    {
        if(dialogTxt == null)
        {
            dialogTxt = UIManager.instance.curUIGroup.childUI[0].GetComponentInChildren<TextMeshProUGUI>(true);
        }

        if(curScarescrow != null)
        {
            UIManager.instance.curUIGroup.transform.position = 
                Camera.main.WorldToScreenPoint(curScarescrow.transform.position) + padding;
        }
    }

    void LoadDialog()
    {
        int curSequence = -1;
        for (int i = 0; i < tutoDB.TutorialEntity.Count; i++)
        {
            curSequence++;
            List<string> tmpDialogs = new List<string>();
            while(curSequence == tutoDB.TutorialEntity[i].sequence)
            {
                tmpDialogs.Add(tutoDB.TutorialEntity[i].dialogue);
                if (tutoDB.TutorialEntity.Count == i+1 || curSequence != tutoDB.TutorialEntity[i + 1].sequence)
                {
                    tutorialDatas.Add(new TutorialData(curSequence, tmpDialogs));
                    break;
                }
                else i++;
            }
        }
    }

    public void LoadEvent(int sequence)
    {   // player가 상호작용한 허수아비에 맞춰 이벤트가 진행됩니다.
        SetScarescrow((ScareScrowType)sequence);
        StartCoroutine(ManageEvent(sequence));
    }

    void SetScarescrow(ScareScrowType type)
    {
        if(type == ScareScrowType.Start) curIdx = SetNextDialog((int)type, curIdx); 
        UIManager.instance.curUIGroup.childUI[0].SetActive(true);

        curScarescrow = scareScrows[(int)type];
        curBoundCollider = boundColliders[(int)type];
    }

    IEnumerator ManageEvent(int sequence)
    {
        yield return new WaitUntil(() => ManageDialog(sequence));
    }

    public int curIdx = 0;
    public bool isFirst = true;
    float time = 0f;
    bool ManageDialog(int sequence)
    {
        if (sequence == 0) time += Time.deltaTime;
        if (sequence == 0 && !isInteraction)
        { 
            if (curIdx <= 1 && time >= 1.5f)
            {
                time = 0f;
                curIdx = SetNextDialog(sequence, curIdx);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {   // 일반 대화 출력
            if(tutorialDatas[sequence].dialogues.Count <= curIdx)
            {
                IsDone();
                return true;
            }
            curIdx = SetNextDialog(sequence, curIdx);
        }
        return false;
    }

    int SetNextDialog(int sequence, int idx)
    {
        if (tutorialDatas[sequence].dialogues.Count > idx)
        {
            dialogTxt.text = tutorialDatas[sequence].dialogues[idx];
            idx++;
        }
        return idx;
    }

    void IsDone()
    {
        UIManager.instance.curUIGroup.childUI[0].SetActive(false);

        curIdx = 0;
        curScarescrow = null;
        isFirst = true;
        curBoundCollider.isTrigger = true;
        GameManager.instance.player.GetComponentInChildren<PlayerInteraction>().dialogueInteraction.isDone = true;
    }
}
