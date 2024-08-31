using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
public class MapIndicator : MonoBehaviour
{
    public GameObject playerUI;
    public GameObject mapUI;
    public GameObject shopUI;
    public GameObject blanketUI;
    RectTransform playerRect, mapRect, shopRect, blanketRect;
    List<RectTransform> mapRects;
    BlockInfo[] blocks;
    bool[] isVisited;
    public int firstBlock;
    [Range(0, 5f)]public float size;

    private void Awake()
    {
    }
    private void OnValidate()
    {
        mapRect = mapUI.GetComponent<RectTransform>();
        blocks = FindObjectsOfType<BlockInfo>();
        isVisited = new bool[blocks.Length];

        for (int i = 0; i < blocks.Length; i++)
        {
            PolygonCollider2D tmpColl = blocks[i].GetComponentInChildren<PolygonCollider2D>();
            Vector3 rectViewPos = blocks[i].transform.position * size;

            RectTransform tmp = Instantiate(mapRect, rectViewPos + transform.position, Quaternion.identity, this.transform);
            tmp.name = "blockUI - " + i;
            tmp.sizeDelta = tmpColl.bounds.size * size;
        }
    }

    void Start()
    {
        //MoveBlockPlayer(firstBlock);  // First Setting
        
    }
    //public void MoveBlockPlayer(int goToBlockNum)   // goToBlockNum is a Hierarchy Order
    //{   
    //    // Set a Player UI
    //    playerUI.transform.SetParent(blockUIInfos[goToBlockNum].transform);
    //    playerUI.transform.localPosition = Vector3.one;

    //    if (isVisited[goToBlockNum]) return;
    //    isVisited[goToBlockNum] = true;

    //    // Inactive a Question mark
    //    blockUIInfos[goToBlockNum].SwitchChild(BlockChild.Question, false);

    //    ShowNextBlock(goToBlockNum);
    //}

    //void ShowNextBlock(int _blockOrder)
    //{   // Active Next Blocks & bridges
    //    if (blockUIInfos[_blockOrder].nextBlock.Length == 0) return;

    //    if (blockUIInfos[_blockOrder].isWay) blockUIInfos[_blockOrder].SwitchChild(BlockChild.Way, true);
    //    foreach (BlockUIInfo nextBlockUI in blockUIInfos[_blockOrder].nextBlock)
    //    {
    //        nextBlockUI.SwitchChild(BlockChild.BG, true);
    //        nextBlockUI.SwitchChild(BlockChild.Question, true);
    //    }
    //}
    //public void BlockClear(int curBlock)
    //{   // Active a Next Interaction Object - shop, boss, blanket
    //    if (blockUIInfos[curBlock].nextBlock.Length == 0) return;

    //    foreach (BlockUIInfo nextBlockUI in blockUIInfos[curBlock].nextBlock)
    //    {
    //        if(!nextBlockUI.isInteraction) continue;

    //        nextBlockUI.SwitchChild(BlockChild.Question, false);
    //        nextBlockUI.SwitchChild(BlockChild.Interaction, true);
    //    }
    //}
}
