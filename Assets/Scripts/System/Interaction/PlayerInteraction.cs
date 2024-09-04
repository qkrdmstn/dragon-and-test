using System.Collections;
using System.Collections.Generic;
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
        if (dialogueInteraction.isDone || blanketInteraction.isDone || pickupItemInteraction.isDone || puzzleInteraction.isDone || shopInteraction.isDone)
        {
            Player.instance.ChangePlayerInteractionState(false);
            pickupItemInteraction.isDone = dialogueInteraction.isDone = blanketInteraction.isDone = puzzleInteraction.isDone = shopInteraction.isDone = false; // 바꿔주지 않으면 해당 조건문 계속 호출...
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
            }
        }
    }

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

            if (interaction.type == InteractionData.InteractionType.Blanket)
            {
                if(!IsClearBlanket(interaction)) return;
            }
            else if(interaction.type == InteractionData.InteractionType.Tutorial && interaction.eventName == "blanket")
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
        obj.GetComponent<Renderer>().material.SetFloat("_isInteraction", isOn);
    }

    private void BlanketDoInteraction()
    {
        BlanketInteractionData data = interaction as BlanketInteractionData;
        if (data.isActive && data.isClear)
        {
            data.isActive = false;
            blanketInteraction.LoadEvent();
        }
        else
        {
            blanketInteraction.isDone = true;
            return;
        }
    }

    TutorialInteraction tuto;
    private void TutorialInteraction()
    {
        if(tuto == null)
            tuto = GameObject.Find("System").GetComponent<TutorialInteraction>();

        if (interaction.eventName == "족보")
        {
            tuto.jokboUIGroup.isPossibleJokbo = true;
            Player.instance.ChangePlayerInteractionState(false);
            return;
        }

        else if(interaction.eventName == "blanket" && interaction.sequence > 0)
        {
            global::TutorialInteraction.isBlanketInteration = true;
            BlanketDoInteraction();
        }

        if (interaction.sequence == 0 && tuto.curIdx >= 2)
        {
            tuto.isInteraction = true;
            Player.instance.ChangePlayerInteractionState(false);
        }
        else if (!tuto.checkSequenceDone[interaction.sequence] &&
            interaction.sequence > 0 && !tuto.isInteraction && tuto.curIdx == 0)
                tuto.LoadEvent(interaction);

        else Player.instance.ChangePlayerInteractionState(false);
    }

    void PuzzleInteraction()
    {    // lever 가동 모션
        puzzleInteraction.LoadEvent(interaction);
    }
}
