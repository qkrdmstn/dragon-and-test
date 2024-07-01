using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public InteractionData interaction;
    public Player player;

    bool isBounded;
    int curIdxInteraction;

    public Interaction dialogueInteraction, shopInteraction, blanketInteraction, pickupItemInteraction;

    CircleCollider2D col;
    Collider2D[] inRangeInteraction;

    [SerializeField] LayerMask layer;
    //DialogueInteraction dialogueInteraction;

    void Awake()
    {
        isBounded = false;

        player = GetComponentInParent<Player>();
        col = GetComponent<CircleCollider2D>();
        dialogueInteraction = gameObject.AddComponent<DialogueInteraction>();
        shopInteraction = gameObject.AddComponent<ShopInteraction>();
        blanketInteraction = gameObject.AddComponent<BlanketInteraction>();
        pickupItemInteraction = gameObject.AddComponent<PickUpItemInteraction>();
    }

    void Update()
    {
        if (isBounded && Input.GetKeyDown(KeyCode.F)) DoInteraction();
        if (dialogueInteraction.isDone || blanketInteraction.isDone || pickupItemInteraction.isDone)
        {
            ChangePlayerInteractionState(false);
            pickupItemInteraction.isDone = dialogueInteraction.isDone = blanketInteraction.isDone = false; // 바꿔주지 않으면 해당 조건문 계속 호출...
        }
    }

    public void ChangePlayerInteractionState(bool state)
    {
        player.isInteraction = state;   // player의 상호작용 여부 관찰
        if (state) player.SetIdleStatePlayer();

        player.isStateChangeable = !state;
        player.isAttackable = !state;
    }

    void DoInteraction()
    {
        if (!player.isInteraction)
        {
            ChangePlayerInteractionState(true);

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
                    // ToDo ITEM INTERACTION
                    dialogueInteraction.LoadEvent(interaction);
                    shopInteraction.LoadEvent(interaction);
                    break;
                case InteractionData.InteractionType.Blanket:
                    BlanketDoInteraction();
                    break;
                case InteractionData.InteractionType.Tutorial:
                    TutorialInteaction();
                    break;

            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Interaction"))
        {
            isBounded = true;
            // 근처 상호작용물 검사

            inRangeInteraction = Physics2D.OverlapCircleAll(transform.position, col.radius, layer);

            curIdxInteraction = 0;
            float minDistance = Vector2.Distance(player.transform.position, inRangeInteraction[0].transform.position);

            for (int i = 1; i < inRangeInteraction.Length; i++)
            {
                //Debug.Log(inRangeInteraction[i].gameObject.name);

                float tmp = minDistance;
                minDistance = Mathf.Min(minDistance, Vector2.Distance(player.transform.position, inRangeInteraction[i].transform.position));
                if (tmp != minDistance)
                {
                    curIdxInteraction = i;
                }
            }

            interaction = inRangeInteraction[curIdxInteraction].gameObject.GetComponent<InteractionData>();
            Debug.Log(inRangeInteraction[curIdxInteraction].gameObject.name);

            inRangeInteraction[curIdxInteraction].gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;

            for (int i = 0; i < inRangeInteraction.Length; i++)
            {   // 제일 가까운 색외에는 다 Hover effect X
                if (i == curIdxInteraction)
                {
                    inRangeInteraction[curIdxInteraction].gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
                }
                else inRangeInteraction[i].gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interaction"))
        {
            isBounded = false;
            interaction = null;

            //inRangeInteraction = Physics2D.OverlapCircleAll(transform.position, col.radius, layer);
            for (int i=0; i<inRangeInteraction.Length; i++)
            {
                inRangeInteraction[i].gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
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

    Tutorial tuto;
    private void TutorialInteaction()
    {
        if(tuto == null)
            tuto = GameObject.Find("System").GetComponent<Tutorial>();

        if (interaction.eventName == "족보")
        {
            Tutorial.getJokbo = true;
            ChangePlayerInteractionState(false);
            return;
        }
        else if (interaction.eventName == "모포")
        {
            Tutorial.FindBlankBullet();
            Tutorial.isBlankBulletCard = true;
            ChangePlayerInteractionState(false);
            return;
        }

        if (interaction.sequence == 0 && tuto.curIdx >= 2)
        {
            tuto.isInteraction = true;
            ChangePlayerInteractionState(false);
        }
        else if (!tuto.checkSequenceDone[interaction.sequence] &&
            interaction.sequence > 0 && !tuto.isInteraction)
                tuto.LoadEvent(interaction.sequence);

        else ChangePlayerInteractionState(false);
    }
}
