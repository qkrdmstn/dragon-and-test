using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    protected InteractionData interation;
    protected Player player;

    DialogueInteraction dialogueInteraction;

    protected void Awake()
    {
        player = GetComponentInParent<Player>();
        dialogueInteraction = new DialogueInteraction();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interaction"))
        {
            interation = collision.gameObject.GetComponent<InteractionData>();
            
            SetInteractionFlag(true);   // 상호작용 시작
            // Debug.Log(interation.gameObject.name);
            switch (interation.type) {
                case InteractionData.InteractionType.NPC:
                    // ToDo DIALOGUE INTERATION : event name 변수로 알맞은 .csv load
                    dialogueInteraction.interation = interation;
                    dialogueInteraction.player = player;

                    dialogueInteraction.LoadEvent();
                    break;
                case InteractionData.InteractionType.Item:
                    // ToDo ITEM INTERQATION
                    break;
            }
        }
    }
    protected virtual void LoadEvent() { }
    protected void SetInteractionFlag(bool _isInteration)
    {
        player.isInteraction = _isInteration;
    }
}
