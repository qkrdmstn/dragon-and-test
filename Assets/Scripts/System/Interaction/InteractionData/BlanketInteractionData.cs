using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlanketInteractionData : InteractionData
{
    public Spawner spawner;
    public bool isClear;
    private void Start()
    {
        spawner = GameObject.FindObjectOfType<Spawner>();
    }

    private void Update()
    {
        if(isClear != spawner.waveEnd)
            isClear = spawner.waveEnd;
    }
}
