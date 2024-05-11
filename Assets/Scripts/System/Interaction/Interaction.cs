using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public bool isDone;

    public virtual void LoadEvent() { }
    public virtual void LoadEvent(InteractionData data) { }

}
