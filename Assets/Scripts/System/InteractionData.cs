using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionData : MonoBehaviour
{
    public enum InteractionType
    {
        NPC,
        Item
    };

    public InteractionType type;
    public string eventName;
}
