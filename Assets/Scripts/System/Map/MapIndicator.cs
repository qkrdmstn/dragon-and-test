using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class MapIndicator : MonoBehaviour
{
    public GameObject playerUI;
    public GameObject mapUI;
    public GameObject shopUI;
    public GameObject blanketUI;
    public GameObject bossUI;
    RectTransform playerRect, mapRect, shopRect, blanketRect, bossRect;
    List<MapInfo> mapRects;

    Spawner spanwner;
    public List<BlockInfo> blocks;

    bool[] isVisited;
    public bool isOverlaped = false;
    [Header("Blocks Information")]
    public int startBlock;
    public int endBlock;
    public int[] blanketBlockNum;
    public int[] shopBlockNum;
    public int curBlock;
    public Vector3 bossUIPos;
    [Range(1, 2f)]public float size;

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

    private void Awake()
    {
        mapRects = new List<MapInfo>();
        mapRect = mapUI.GetComponent<RectTransform>();
        spanwner = GameObject.Find("Spawner").GetComponent<Spawner>();
    }
    private void Start()
    {
        blocks = spanwner.blocks;
        isVisited = new bool[blocks.Count];
        curBlock = startBlock;
        InstantiateBlockUI();
    }

    void InstantiateBlockUI()
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
                bossRect.sizeDelta = new Vector2(size * 25, size * 25);
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
            else rectSize.x = size * 25;

            if (tmpColl.bounds.size.y >= 35) rectSize.y = size * 52f;
            else rectSize.y = size * 25;

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
        for (int i = 0; i < blanketBlockNum.Length; i++)
        {
            Instantiate(blanketUI, mapRects[blanketBlockNum[i]].mapRect.transform.position, Quaternion.identity, mapRects[blanketBlockNum[i]].mapRect.transform);
        }
        for (int i=0; i<shopBlockNum.Length; i++)
        {
            Instantiate(shopUI, mapRects[shopBlockNum[i]].mapRect.transform.position, Quaternion.identity, mapRects[shopBlockNum[i]].mapRect.transform);
        }
        playerRect = Instantiate(playerUI, mapRects[startBlock].mapRect.transform.position, Quaternion.identity, mapRects[startBlock].mapRect.transform).GetComponent<RectTransform>();

        SetInActiveAllBlockUIs();
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

        if (isVisited[goToBlockNum]) return;
        isVisited[goToBlockNum] = true;

        FindNearBlocks(goToBlockNum);
    }

    void OverlapPlayerUI()
    {   // 플레이어 UI에 따른 모포, 상점 UI 활성화 상태 변경
        if (mapRects[curBlock].mapRect.childCount > 2)
        {   // BG + Shop || blanket + player
            mapRects[curBlock].mapRect.GetChild(1).gameObject.SetActive(false);
            isOverlaped = true;
        }
    }
    

    void SetInActiveAllBlockUIs()
    {   // 초기 시작을 제외한 나머지 block 비활성화
        for (int i = 0; i < mapRects.Count; i++)
        {
            if (i == startBlock) continue;
            SetInActiveBlockUI(i, false);
        }
        MoveBlockPlayer(startBlock);
    }

    void SetInActiveBlockUI(int blockNum, bool state)
    {
        for (int j = 0; j < mapRects[blockNum].mapRect.childCount; j++)
        {
            mapRects[blockNum].mapRect.GetChild(j).gameObject.SetActive(state);
        }
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

        for (int i = 0; i < tmp.Length; i++)
        {   // index 주의할 것
            int num = int.Parse(tmp[i].gotoMapType.name.Substring(5))-1; // hierarchy는 1부터 시작하므로 보정
            SetInActiveBlockUI(num, true);
        }
    }
}
