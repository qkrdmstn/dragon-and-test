using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public InteractionData interaction;

    //public bool isBounded;
    int curIdxInteraction;

    public Interaction dialogueInteraction, shopInteraction, blanketInteraction, pickupItemInteraction, puzzleInteraction;

    CircleCollider2D col;
    Collider2D[] inRangeInteraction;

    [SerializeField] LayerMask layer;

    void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        dialogueInteraction = gameObject.AddComponent<DialogueInteraction>();
        shopInteraction = gameObject.AddComponent<ShopInteraction>();
        blanketInteraction = gameObject.AddComponent<BlanketInteraction>();
        pickupItemInteraction = gameObject.AddComponent<PickUpItemInteraction>();
        puzzleInteraction = gameObject.AddComponent<PuzzleInteraction>();
    }

    void Update()
    {
        if (Player.instance.isBounded && Input.GetKeyDown(KeyCode.F)) DoInteraction();
        if (pickupItemInteraction.isDone || puzzleInteraction.isDone)
        {   // shop blanket dialogue는 uimanger에서 ui 다꺼진 이후 플레이어 상태 조정
            Player.instance.ChangePlayerInteractionState(false);
            pickupItemInteraction.isDone = puzzleInteraction.isDone = false; // 바꿔주지 않으면 해당 조건문 계속 호출...
        }
    }

    void DoInteraction()
    {
        if (!Player.instance.isInteraction)
        {
            Player.instance.ChangePlayerInteractionState(true);

            switch (interaction.type)
            {
                case InteractionData.InteractionType.NPC:
                    // ToDo DIALOGUE INTERACTION
                    dialogueInteraction.LoadEvent(interaction);
                    break;
                case InteractionData.InteractionType.Item:
                    pickupItemInteraction.LoadEvent(interaction);
                    break;
                case InteractionData.InteractionType.Shop:
                    shopInteraction.LoadEvent(interaction);
                    break;
                case InteractionData.InteractionType.Blanket:
                    BlanketDoInteraction();
                    break;
                case InteractionData.InteractionType.Tutorial:
                    TutorialInteraction();
                    break;
                case InteractionData.InteractionType.Puzzle:
                    PuzzleInteraction();
                    break;
                case InteractionData.InteractionType.Boss:
                    BossInteraction();
                    break;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interaction"))
        {
            interaction = collision.gameObject.GetComponent<InteractionData>();
            BossInteractionData bossInteractionData = interaction as BossInteractionData;
            if (interaction.type == InteractionData.InteractionType.Boss && bossInteractionData != null && bossInteractionData.isActive)
            {
                bossInteractionData.IsDone();
                DoInteraction();
            }
        }
    }

    //상점 때문에 stay로 함.
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Interaction"))
        {
            Player.instance.isBounded = true;
            // 근처 상호작용물 검사

            inRangeInteraction = Physics2D.OverlapCircleAll(transform.position, col.radius, layer);

            curIdxInteraction = 0;
            float minDistance = Vector2.Distance(Player.instance.transform.position, inRangeInteraction[0].transform.position);

            for (int i = 1; i < inRangeInteraction.Length; i++)
            {
                float tmp = minDistance;
                minDistance = Mathf.Min(minDistance, Vector2.Distance(Player.instance.transform.position, inRangeInteraction[i].transform.position));
                if (tmp != minDistance)
                {
                    curIdxInteraction = i;
                }
            }
            interaction = inRangeInteraction[curIdxInteraction].gameObject.GetComponent<InteractionData>();

            if (interaction?.type == InteractionData.InteractionType.Blanket)
            {
                if(!IsClearBlanket(interaction)) return;
            }
            else if(interaction?.type == InteractionData.InteractionType.Tutorial && interaction.eventName == "blanket")
            {
                if (!IsClearBlanket(interaction)) return;
            }

            for (int i = 0; i < inRangeInteraction.Length; i++)
            {   // 제일 가까운 색외에는 다 Hover effect X
                if (i == curIdxInteraction)
                {
                    OnOutline(inRangeInteraction[curIdxInteraction].gameObject, 1);
                }
                else OnOutline(inRangeInteraction[curIdxInteraction].gameObject, 0);
            }

        }
    }

    bool IsClearBlanket(InteractionData data)
    {
        if (data.GetComponent<BlanketInteractionData>().isClear) return true;
        else return false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interaction"))
        {
            Player.instance.isBounded = false;
            interaction = null;

            for (int i=0; i<inRangeInteraction.Length; i++)
            {
                OnOutline(inRangeInteraction[i].gameObject, 0); // true = 1, false = 0
            }
        }
    }

    private void OnOutline(GameObject obj, float isOn)
    {
        //호버 효과 필요 없는 object 예외처리
        if (inRangeInteraction[curIdxInteraction].gameObject.GetComponent<Renderer>() == null) return;
        obj.GetComponent<Renderer>().material.SetFloat("_isInteraction", isOn);
    }

    private void BlanketDoInteraction()
    {
        BlanketInteractionData data = interaction as BlanketInteractionData;
        if (data.isActive && data.isClear)
        {   // possible to try only once by blanket
            data.isActive = false;  
            blanketInteraction.LoadEvent();
        }
        else
        {
            Player.instance.ChangePlayerInteractionState(false);
            blanketInteraction.isDone = true;
            return;
        }
    }

    TutorialInteraction tutorialInteraction;
    private void TutorialInteraction()
    {
        if(tutorialInteraction == null)
            tutorialInteraction = GameObject.Find("System").GetComponent<TutorialInteraction>();

        if (tutorialInteraction.interactionF && interaction.eventName == "Jokbo")
        {
            tutorialInteraction.jokboUIGroup.isPossibleJokbo = true;
            Player.instance.ChangePlayerInteractionState(false);
            return;
        }

        else if(interaction.eventName == "blanket" && interaction.sequence > 0)
        {
            global::TutorialInteraction.isBlanketInteration = true;
            BlanketDoInteraction();
        }

        if (interaction.sequence == 0 && tutorialInteraction.curIdx >= 2)
        {
            tutorialInteraction.isInteraction = true;
            Player.instance.ChangePlayerInteractionState(false);
        }
        else if((int)tutorialInteraction.curScarescrowState.type == interaction.sequence && tutorialInteraction.curScarescrowState.isSequenceDone)   // 종료된 허수아비
            Player.instance.ChangePlayerInteractionState(false);

        else if (tutorialInteraction.curScarescrowType == ScareScrowType.None &&
            interaction.sequence > 0 && !tutorialInteraction.isInteraction && tutorialInteraction.curIdx == 0)
            tutorialInteraction.LoadEvent(interaction);

        else Player.instance.ChangePlayerInteractionState(false);
    }

    void PuzzleInteraction()
    {    // lever 가동 모션
        puzzleInteraction.LoadEvent(interaction);
    }

    //보스 방 입장 시 호출
    public void BossInteraction()
    {
        dialogueInteraction.LoadEvent(interaction);
    }
}
