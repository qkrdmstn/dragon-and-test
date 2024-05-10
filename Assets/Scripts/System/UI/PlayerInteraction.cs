using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public InteractionData interaction;
    public Player player;

    bool isBounded;
    int curIdxInteraction;

    Interaction dialogueInteraction, shopInteraction, blanketInteraction;

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
    }

    void Update()
    {
        if (isBounded && Input.GetKeyDown(KeyCode.F)) DoInteraction();
        if (dialogueInteraction.isDone || blanketInteraction.isDone)
        {
            player.isInteraction = false;   // player의 상호작용 여부 관찰
            player.isStateChangeable = true;
            player.isAttackable = true;
        }
    }

    void DoInteraction()
    {
        if (!player.isInteraction)
        {
            player.isInteraction = true;
            player.SetIdleStatePlayer();
            player.isStateChangeable = false;
            player.isAttackable = false;

            switch (interaction.type)
            {
                case InteractionData.InteractionType.NPC:
                    // ToDo DIALOGUE INTERACTION
                    if (interaction.eventName == "튜토리얼")
                    {
                        Debug.Log("interaction scarecrow");
                        // TODO ----- 허수아비와 상호작용했다는 정보를 튜토리얼 함수쪽으로 전달 필요
                        dialogueInteraction.isDone = true;
                        break;
                    }

                    dialogueInteraction.LoadEvent(interaction);

                    break;
                case InteractionData.InteractionType.Item:
                    // ToDo ITEM INTERACTION
                    dialogueInteraction.LoadEvent(interaction);
                    shopInteraction.LoadEvent(interaction);

                    break;
                case InteractionData.InteractionType.Blanket:
                    BlanketDoInteraction();
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
        if (!data.isClear)
        {
            blanketInteraction.isDone = true;
            return;
        }
        else if (data.isActive)
        {
            data.isActive = false;
            blanketInteraction.LoadEvent();
        }
    }
}
