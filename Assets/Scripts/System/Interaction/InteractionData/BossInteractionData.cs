using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossInteractionData : InteractionData
{
    public bool isActive;

    private void Start()
    {
        isActive = true;
    }

    public void IsDone()
    {
        isActive=false;
    }
}
