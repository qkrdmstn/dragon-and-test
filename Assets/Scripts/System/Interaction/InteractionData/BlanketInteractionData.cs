using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlanketInteractionData : InteractionData
{
    public BlockInfo curBlock;
    public GameObject mapBlanketUI; 

    public bool isClear;
    public bool isActive;
    public bool isTutorial;

    private void Start()
    {
        if (isTutorial) return;
        isClear = false;
        isActive = true;
        
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
        else if (isClear != curBlock.blockClear)
            isClear = curBlock.blockClear;

        if (isClear && !isActive)
        {   //모포 사용 후, 제거
            Destroy(mapBlanketUI);  // 미니맵의 모포 UI도 같이 삭제
            MapIndicator.DecreaseActiveBlanketUI();

            Destroy(this.gameObject);
        }
    }
}
