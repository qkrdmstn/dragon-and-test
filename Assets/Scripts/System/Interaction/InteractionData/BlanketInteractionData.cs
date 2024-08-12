using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlanketInteractionData : InteractionData
{
    public Spawner spawner;
    public BlockInfo curBlock;
    public bool isClear;
    public bool isActive;
    public bool isTutorial;

    private void Start()
    {
        if (isTutorial) return;
        isClear = false;
        isActive = true;
        
        spawner = GameObject.FindObjectOfType<Spawner>();
        BlockInfo[] blocks = FindObjectsOfType<BlockInfo>();
        for(int i=0; i<blocks.Length; i++)
        {
            if (blocks[i].IsInBlock(this.transform.position))
            {
                curBlock = blocks[i];
                break;
            }
        }
    }

    private void Update()
    {
        if (isTutorial) return;

        if (curBlock == null)
            isClear = true;
        else if(isClear != curBlock.blockClear)
            isClear = curBlock.blockClear;
    }
}
