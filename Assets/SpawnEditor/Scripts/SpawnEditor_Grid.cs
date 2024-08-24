using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class SpawnEditor_Grid : MonoBehaviour
{
    public Vector2[] mapHierarchy;
    [SerializeField] private List<SpawnEditor_BlockInfo> blocks;

    [Header("Grid info")]
    [SerializeField] private LineRenderer[] lines;
    int lineCnt = 0;

    // Start is called before the first frame update
    void Awake()
    {
        InitializeBlockInfo();

        lines = GetComponentsInChildren<LineRenderer>();
        InitializeLineRender();
    }

    private void InitializeBlockInfo()
    {
        //blocks = FindObjectsOfType<SpawnEditor_BlockInfo>();
        //Array.Sort(blocks);

        //mapIndicator = FindObjectOfType<MapIndicator>();
        int layer = 1 << LayerMask.NameToLayer("MapSort");
        for (int i = 0; i < mapHierarchy.Length; i++)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(mapHierarchy[i], Vector2.right, 250f, layer);
            foreach (RaycastHit2D hit in hits)
            {
                blocks.Add(hit.transform.GetComponentInParent<SpawnEditor_BlockInfo>());
            }
        }
        blocks = blocks.Distinct().ToList();

        for (int i = 0; i < blocks.Count; i++)
            blocks[i].InitializeBlockInfo(i);
    }

    private void InitializeLineRender()
    {
        lineCnt = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].startColor = lines[i].endColor = Color.cyan;
            lines[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
        InitializeLineRender();
        if (SpawnEditorManager.instance.curState == SpawnEditor_State.BlockSetting)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            for (int i = 0; i < blocks.Count; i++)
            {
                Color lineColor = Color.cyan;
                if (blocks[i].IsInBlock(mousePos))
                    lineColor = Color.red;

                Vector3 pos1 = blocks[i].min;
                Vector3 pos2 = new Vector3(blocks[i].max.x, blocks[i].min.y, 0);
                lines[lineCnt].SetPosition(0, pos1);
                lines[lineCnt].SetPosition(1, pos2);
                lines[lineCnt].startWidth = lines[lineCnt].endWidth = 1.0f;
                lines[lineCnt].startColor = lines[lineCnt].endColor = lineColor;
                lines[lineCnt].gameObject.SetActive(true);
                lineCnt++;

                pos2 = new Vector3(blocks[i].min.x, blocks[i].max.y, 0);
                lines[lineCnt].SetPosition(0, pos1);
                lines[lineCnt].SetPosition(1, pos2);
                lines[lineCnt].startWidth = lines[lineCnt].endWidth = 1.0f;
                lines[lineCnt].startColor = lines[lineCnt].endColor = lineColor;
                lines[lineCnt].gameObject.SetActive(true);
                lineCnt++;

                pos1 = blocks[i].max;
                pos2 = new Vector3(blocks[i].max.x, blocks[i].min.y, 0);
                lines[lineCnt].SetPosition(0, pos1);
                lines[lineCnt].SetPosition(1, pos2);
                lines[lineCnt].startWidth = lines[lineCnt].endWidth = 1.0f;
                lines[lineCnt].startColor = lines[lineCnt].endColor = lineColor;
                lines[lineCnt].gameObject.SetActive(true);
                lineCnt++;

                pos2 = new Vector3(blocks[i].min.x, blocks[i].max.y, 0);
                lines[lineCnt].SetPosition(0, pos1);
                lines[lineCnt].SetPosition(1, pos2);
                lines[lineCnt].startWidth = lines[lineCnt].endWidth = 1.0f;
                lines[lineCnt].startColor = lines[lineCnt].endColor = lineColor;
                lines[lineCnt].gameObject.SetActive(true);
                lineCnt++;
            }
        }
        else if(SpawnEditorManager.instance.curState == SpawnEditor_State.WaveSetting)
        {
            SpawnEditor_BlockInfo selectedBlock = SpawnEditorManager.instance.selectedBlock;
            DrawHorizontalLine(selectedBlock);
            DrawVerticalLine(selectedBlock);
        }
    }

    private void DrawHorizontalLine(SpawnEditor_BlockInfo selectedBlock)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPos = new Vector2Int(-1,-1);
        if (selectedBlock.IsInBlock(mousePos))
            gridPos = selectedBlock.WorldToGridPosition(mousePos);
        int upIdx = gridPos.y + 1;
        int downIdx = gridPos.y;

        int cnt = 0;
        //Horizontal
        Vector3 pos1 = selectedBlock.min;
        Vector3 pos2 = new Vector3(selectedBlock.max.x, selectedBlock.min.y, 0);
        while (selectedBlock.IsInBlock(pos2))
        {
            lines[lineCnt].SetPosition(0, pos1);
            lines[lineCnt].SetPosition(1, pos2);
            lines[lineCnt].startWidth = lines[lineCnt].endWidth = 0.1f;

            Color lineColor = Color.cyan;
            if(cnt == upIdx || cnt == downIdx)
                lines[lineCnt].startColor = lines[lineCnt].endColor = Color.red;
  
            lines[lineCnt].gameObject.SetActive(true);

            pos1 += new Vector3(0, selectedBlock.gridSize.y, 0);
            pos2 += new Vector3(0, selectedBlock.gridSize.y, 0);
            lineCnt++;
            cnt++;
        }
    }

    private void DrawVerticalLine(SpawnEditor_BlockInfo selectedBlock)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPos = new Vector2Int(-1, -1);
        if (selectedBlock.IsInBlock(mousePos))
            gridPos = selectedBlock.WorldToGridPosition(mousePos);
        int rightIdx = gridPos.x + 1;
        int leftIdx = gridPos.x;

        int cnt = 0;
        //Vertical
        Vector3 pos1 = selectedBlock.min;
        Vector3 pos2 = new Vector3(selectedBlock.min.x, selectedBlock.max.y, 0);
        while (selectedBlock.IsInBlock(pos2))
        {
            lines[lineCnt].SetPosition(0, pos1);
            lines[lineCnt].SetPosition(1, pos2);
            lines[lineCnt].startWidth = lines[lineCnt].endWidth = 0.1f;
            Color lineColor = Color.cyan;
            if (cnt == leftIdx || cnt == rightIdx)
                lines[lineCnt].startColor = lines[lineCnt].endColor = Color.red;

            lines[lineCnt].gameObject.SetActive(true);
            pos1 += new Vector3(selectedBlock.gridSize.x, 0, 0);
            pos2 += new Vector3(selectedBlock.gridSize.x, 0, 0);
            lineCnt++;
            cnt++;
        }
    }
}
