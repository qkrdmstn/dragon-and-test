using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class MapIndicator : MonoBehaviour
{
    public GameObject playerUI;
    public GameObject mapUI;
    public GameObject shopUI;
    public GameObject blanketUI;
    public GameObject puzzleUI;
    public GameObject bossUI;
    RectTransform playerRect, mapRect, bossRect, puzzleRect;
    List<MapInfo> mapRects;

    RectTransform panel;
    Spawner spanwner;
    List<int> curNearBlocksNum;
    public List<BlockInfo> blocks;

    bool[] isVisited;
    public bool isOverlaped = false;

    [Header("Blocks Information")]
    public int startBlock;
    public int curBlock;
    public int endBlock;
    public int[] shopBlockNum;

    public Vector3 bossUIPos, puzzleUIPos;
    [Range(25, 50f)] public int defaultSize;
    [Range(1, 2f)] public float size;
    Vector2 activePosforMoney, inactivePosforMoney;

    #region DELEGATE_MOVE
    public delegate void MoveMap(bool state);
    public MoveMap onMoveMap;
    #endregion

    public class MapInfo
    {
        public RectTransform mapRect;
        public MapControl[] gotoMaps;
        public MapInfo(RectTransform mapRect, MapControl[] gotoMaps)
        {
            this.mapRect = mapRect;
            this.gotoMaps = gotoMaps;
        }
    }

    void Awake()
    {
        panel = transform.parent.GetComponent<RectTransform>();
        activePosforMoney = new Vector2(panel.anchoredPosition.x, -panel.rect.height - 10f);
        inactivePosforMoney = new Vector2(panel.anchoredPosition.x, 0f);

        mapRects = new List<MapInfo>();
        curNearBlocksNum = new List<int>();

        curActiveBlanketCntTxt = GetComponentInChildren<TextMeshProUGUI>();
        mapRect = mapUI.GetComponent<RectTransform>();
        spanwner = GameObject.Find("Spawner").GetComponent<Spawner>();
    }
    void Start()
    {
        blocks = spanwner.blocks;
        isVisited = new bool[blocks.Count];

        onMoveMap = SwitchMapPanel;
        onMoveMap += UpdateMoneyPos;

        curBlock = startBlock;
        InstantiateBattleBlockUI();
    }

    #region Puzzle
    void InitializePuzzleBlock()
    {
        puzzleRect = Instantiate(puzzleUI, Vector3.zero, Quaternion.identity, this.transform).GetComponent<RectTransform>();
        puzzleRect.sizeDelta = new Vector2(size * defaultSize, size * defaultSize);
        puzzleRect.anchoredPosition = puzzleUIPos;

        bossRect = Instantiate(bossUI, Vector3.zero, Quaternion.identity, this.transform).GetComponent<RectTransform>();
        bossRect.sizeDelta = new Vector2(size * defaultSize, size * defaultSize);
        bossRect.anchoredPosition = bossUIPos;

        //playerRect = Instantiate(playerUI, puzzleRect.position, Quaternion.identity, puzzleRect.transform).GetComponent<RectTransform>();  // choice
    }
    #endregion

    #region Battle
    void InstantiateBattleBlockUI()
    {
        for (int i = 0; i < blocks.Count; i++)
        {   // spawner에서 정렬된 block을 기준으로 미니맵 Rect를 생성함
            // also loaded spawnZones of each block

            Vector3 rectViewPos = blocks[i].transform.position * size;
            RectTransform tmp = Instantiate(mapRect, rectViewPos + transform.position, Quaternion.identity, this.transform);
            tmp.name = "blockUI - " + i;

            if (i == endBlock)
            {   // 마지막 block은 boss까지 연계해서 생성
                bossRect = Instantiate(bossUI, Vector3.one, Quaternion.identity, this.transform).GetComponent<RectTransform>();
                bossRect.sizeDelta = new Vector2(size * defaultSize, size * defaultSize);
                bossRect.anchoredPosition = bossUIPos;
                for (int j = 0; j < bossRect.childCount; j++)
                {
                    bossRect.GetChild(j).gameObject.SetActive(false);
                }
            }

            // 크기 보정
            PolygonCollider2D tmpColl = blocks[i].GetComponentInChildren<PolygonCollider2D>();
            Vector2 rectSize = Vector2.one;
            if (tmpColl.bounds.size.x >= 35) rectSize.x = size * 52f;
            else rectSize.x = size * defaultSize;

            if (tmpColl.bounds.size.y >= 35) rectSize.y = size * 52f;
            else rectSize.y = size * defaultSize;

            tmp.sizeDelta = rectSize;
            tmp.GetComponent<BoxCollider2D>().size = rectSize;

            // 현재 block에서 갈수있는 경로 안내를 위한 mapControl 가져오기
            MapInfo mapInfo = new MapInfo(tmp, blocks[i].GetComponentsInChildren<MapControl>());
            mapRects.Add(mapInfo);
        }
        InitailizeUI();
    }

    void InitailizeUI()
    {
        BlanketInteractionData[] datas = FindObjectsOfType<BlanketInteractionData>();
        curActiveBlanketCnt = datas.Length;
        SetBlanketUICnt();

        foreach (BlanketInteractionData data in datas)
        {   // 추후 각자 모포 isActive == false -> destroy
            data.mapBlanketUI = Instantiate(blanketUI, mapRects[data.curBlock.blockNumber].mapRect.transform.position, Quaternion.identity, mapRects[data.curBlock.blockNumber].mapRect.transform);
        }

        for (int i = 0; i < shopBlockNum.Length; i++)
        {
            GameObject shopObj = Instantiate(shopUI, mapRects[shopBlockNum[i]].mapRect.transform.position, Quaternion.identity, mapRects[shopBlockNum[i]].mapRect.transform);
            shopObj.SetActive(false);
        }
        playerRect = Instantiate(playerUI, mapRects[startBlock].mapRect.transform.position, Quaternion.identity, mapRects[startBlock].mapRect.transform).GetComponent<RectTransform>();
        mapRects[startBlock].mapRect.GetComponentInChildren<Animator>().SetBool("isOn", false);

        SetInActiveAllBlockUIs();
    }

    void OverlapPlayerUI()
    {   // 플레이어 UI에 따른 모포, 상점 UI 활성화 상태 변경
        if (mapRects[curBlock].mapRect.childCount > 2)
        {   // BG + Shop || blanket + player
            mapRects[curBlock].mapRect.GetChild(1).gameObject.SetActive(false);
            isOverlaped = true;
        }
    }

    public void MoveBlockPlayer(int goToBlockNum)   // goToBlockNum is a Hierarchy Order
    {
        if (isOverlaped && curBlock != goToBlockNum)
        {
            mapRects[curBlock].mapRect.GetChild(1).gameObject.SetActive(true);
            isOverlaped = false;
        }
        curBlock = goToBlockNum;

        playerRect.transform.SetParent(mapRects[goToBlockNum].mapRect.transform);
        playerRect.parent.GetChild(0).gameObject.SetActive(true);       // 자기자신 BG 활성화
        playerRect.anchoredPosition = Vector2.zero;

        OverlapPlayerUI();
        FindNearBlocks(goToBlockNum);
    }

    public void SwitchMapPanel(bool state) => transform.parent.gameObject.SetActive(state);
    public void UpdateMoneyPos(bool state)
    {   // playerPresenter의 money UI
        UIManager.instance.presenters[0].objs[1].GetComponent<RectTransform>().anchoredPosition
        = state ? activePosforMoney : inactivePosforMoney;
    }

    void FindNearBlocks(int _curBlock)
    {   // 현재 맵의 이동 포탈과 연계해 idx에 맞춰 다음 맵 활성화
        MapControl[] tmp = mapRects[_curBlock].gotoMaps;
        if (_curBlock == endBlock)
        {
            for (int j = 0; j < bossRect.childCount; j++)
            {   // boss 가기 직전 block이라면 bossRect 활성화
                bossRect.GetChild(j).gameObject.SetActive(true);
            }
        }

        if (curNearBlocksNum.Count > 0)
        {
            BlinkBlock(false);
            foreach (int num in curNearBlocksNum)
            {
                if (isVisited[num] || num == _curBlock) continue;
                SetInActiveBlockUI(num, false);
            }
            curNearBlocksNum.Clear();
        }

        for (int i = 0; i < tmp.Length; i++)
        {   // index 주의할 것
            int num = int.Parse(tmp[i].gotoMapType.name.Substring(5))-1; // hierarchy는 1부터 시작하므로 보정
            if (!isVisited[num])
                curNearBlocksNum.Add(num);

            SetInActiveBlockUI(num, true);
        }

        BlinkBlock(isVisited[_curBlock]);   // block의 clear상태에 맞춰 깜빡임 활성화 결정
        isVisited[_curBlock] = true;        
    }

    void SetInActiveAllBlockUIs()
    {   // 초기 시작을 제외한 나머지 block 비활성화
        for (int i = 0; i < mapRects.Count; i++)
        {
            if (i == startBlock) continue;
            SetInActiveBlockUI(i, false);
        }
    }

    void SetInActiveBlockUI(int blockNum, bool state)
    {
        foreach (int shopNum in shopBlockNum)
        {   if (blockNum == shopNum)
            {
                if (isVisited[blockNum])
                {   // 상점을 방문한 뒤에만 UI 활성화
                    mapRects[blockNum].mapRect.GetChild(1).gameObject.SetActive(state);
                }
                else
                {   // 미방문이었다면 BG만 활성화
                    mapRects[blockNum].mapRect.GetChild(0).gameObject.SetActive(state);
                }
                return;
            }
        }

        for (int j = 0; j < mapRects[blockNum].mapRect.childCount; j++)     // 상점이 아닌 경우,
            mapRects[blockNum].mapRect.GetChild(j).gameObject.SetActive(state);
    }

    void OnDestroy()
    {   // playerPresenter의 money UI
        UIManager.instance.presenters[0].objs[1].GetComponent<RectTransform>().anchoredPosition
                = inactivePosforMoney;
    }

    public void BlinkBlock(bool state)
    {
        onMoveMap(state);
        if (state && curBlock == endBlock)
        {
            Animator anim = bossRect.GetComponentInChildren<Animator>();
            anim.SetBool("isOn", state);
        }

        foreach (int num in curNearBlocksNum)
        {
            Animator anim = mapRects[num].mapRect.GetComponentInChildren<Animator>();
            anim.SetBool("isOn", state);
        }
    }

    public static TextMeshProUGUI curActiveBlanketCntTxt;
    public static int curActiveBlanketCnt;
    public static void SetBlanketUICnt() => curActiveBlanketCntTxt.text = "X " + curActiveBlanketCnt;
    #endregion
}
