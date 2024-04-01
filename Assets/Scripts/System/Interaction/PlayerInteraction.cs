using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public InteractionData interaction;
    public Player player;

    bool isBounded;
    int curIdxInteraction;

    Interaction dialogueInteraction;
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
    }

    void Update()
    {
        if (isBounded && Input.GetKeyDown(KeyCode.Space)) DoInteraction();
        if (dialogueInteraction.isDone) player.isInteraction = false;   // player의 상호작용 여부 관찰
    }

    void DoInteraction()
    {
        switch (interaction.type)
        {
            case InteractionData.InteractionType.NPC:
                // ToDo DIALOGUE INTERACTION
                if (!player.isInteraction)
                {
                    player.isInteraction = true;
                    dialogueInteraction.LoadEvent(interaction.eventName);
                }
                break;
            case InteractionData.InteractionType.Item:
                // ToDo ITEM INTERACTION
                break;
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
                Debug.Log(inRangeInteraction[i].gameObject.name);

                float tmp = minDistance;
                minDistance = Mathf.Min(minDistance, Vector2.Distance(player.transform.position, inRangeInteraction[i].transform.position));
                if (tmp != minDistance)
                {
                    curIdxInteraction = i;
                }
            }

            interaction = inRangeInteraction[curIdxInteraction].gameObject.GetComponent<InteractionData>();
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

}
