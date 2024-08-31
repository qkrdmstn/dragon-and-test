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

    Spawner spanwner;
    List<BlockInfo> blocks;

    bool[] isVisited;
    [Header("Blocks Information")]
    public int firstBlock;
    public int[] blanketBlockNum;
    public int[] shopBlockNum;
    [Range(0, 5f)]public float size;

    private void Awake()
    {
        mapRects = new List<RectTransform>();
        mapRect = mapUI.GetComponent<RectTransform>();
        spanwner = GameObject.Find("Spawner").GetComponent<Spawner>();
    }
    private void Start()
    {
        blocks = spanwner.blocks;
        for (int i = 0; i < blocks.Count; i++)
        {    // spawner에서 정렬된 block을 기준으로 미니맵 Rect를 생성함
            PolygonCollider2D tmpColl = blocks[i].GetComponentInChildren<PolygonCollider2D>();
            Vector3 rectViewPos = blocks[i].transform.position * size;

            RectTransform tmp = Instantiate(mapRect, rectViewPos + transform.position, Quaternion.identity, this.transform);
            tmp.name = "blockUI - " + i;

            Vector2 rectSize = Vector2.one;
            if (tmpColl.bounds.size.x * size >= 80) rectSize.x = 100;
            else rectSize.x = 50;

            if (tmpColl.bounds.size.y * size >= 80) rectSize.y = 100;
            else rectSize.y = 50;

            tmp.sizeDelta = rectSize;
            Debug.Log(tmp.name+ "            "+tmp.sizeDelta);

            mapRects.Add(tmp);
        }
        isVisited = new bool[blocks.Count];
        InitailizeUI();
        //MoveBlockPlayer(firstBlock);  // First Setting
    }

    void InitailizeUI()
    {
        playerRect = Instantiate(playerUI, mapRects[firstBlock].transform.position, Quaternion.identity, mapRects[firstBlock].transform).GetComponent<RectTransform>();
        for (int i = 0; i < blanketBlockNum.Length; i++)
        {
            Instantiate(blanketUI, mapRects[blanketBlockNum[i]].transform.position, Quaternion.identity, mapRects[blanketBlockNum[i]].transform);
        }
        for (int i=0; i<shopBlockNum.Length; i++)
        {
            Instantiate(shopUI, mapRects[shopBlockNum[i]].transform.position, Quaternion.identity, mapRects[shopBlockNum[i]].transform);
        }
    }

    public void MoveBlockPlayer(int goToBlockNum)   // goToBlockNum is a Hierarchy Order
    {
        // Set a Player UI
        playerUI.transform.SetParent(mapRects[goToBlockNum].transform);
        playerUI.transform.localPosition = Vector3.zero;

        if (isVisited[goToBlockNum]) return;
        isVisited[goToBlockNum] = true;

        // Inactive a Question mark
        //blockUIInfos[goToBlockNum].SwitchChild(BlockChild.Question, false);

        //ShowNextBlock(goToBlockNum);
    }

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
