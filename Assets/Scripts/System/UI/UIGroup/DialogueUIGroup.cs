using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUIGroup : UIGroup
{
    DialogueInteraction dialogueInteraction;
    public bool isExit = false;
    public void EndInteraction()
    {
        if (!isExit) isExit = true;
    }
}
