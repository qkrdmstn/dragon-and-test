using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapIndicator : MonoBehaviour
{
    [SerializeField] GameObject playerUI;
    BlockUIInfo[] blockUIInfos;
    bool[] isVisited;
    public int firstBlock;

    void Awake()
    {
        blockUIInfos = GetComponentsInChildren<BlockUIInfo>();
        Array.Sort(blockUIInfos);   // ordered by pos

        isVisited = new bool[blockUIInfos.Length];
     }
    void Start()
    {
        MoveBlockPlayer(firstBlock);  // First Setting
    }

    public void MoveBlockPlayer(int goToBlockNum)   // goToBlockNum is a Hierarchy Order
    {   
        // Set a Player UI
        playerUI.transform.SetParent(blockUIInfos[goToBlockNum].transform);
        playerUI.transform.localPosition = Vector3.one;

        if (isVisited[goToBlockNum]) return;
        isVisited[goToBlockNum] = true;

        // Inactive a Question mark
        blockUIInfos[goToBlockNum].SwitchChild(BlockChild.Question, false);

        ShowNextBlock(goToBlockNum);
    }

    void ShowNextBlock(int _blockOrder)
    {   // Active Next Blocks & bridges
        if (blockUIInfos[_blockOrder].nextBlock.Length == 0) return;

        if (blockUIInfos[_blockOrder].isWay) blockUIInfos[_blockOrder].SwitchChild(BlockChild.Way, true);
        foreach (BlockUIInfo nextBlockUI in blockUIInfos[_blockOrder].nextBlock)
        {
            nextBlockUI.SwitchChild(BlockChild.BG, true);
            nextBlockUI.SwitchChild(BlockChild.Question, true);
        }
    }
    public void BlockClear(int curBlock)
    {   // Active a Next Interaction Object - shop, boss, blanket
        if (blockUIInfos[curBlock].nextBlock.Length == 0) return;

        foreach (BlockUIInfo nextBlockUI in blockUIInfos[curBlock].nextBlock)
        {
            if(!nextBlockUI.isInteraction) continue;

            nextBlockUI.SwitchChild(BlockChild.Question, false);
            nextBlockUI.SwitchChild(BlockChild.Interaction, true);
        }
    }
}
