using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInfo : MonoBehaviour, IComparable
{
    public int blockNumber;
    public bool blockClear;
    public Vector2 blockPos;

    [Header("Collider info")]
    [SerializeField] private Vector2 min;
    [SerializeField] private Vector2 max;
    [SerializeField] private Vector2 blockSize;
    
    [Header("Grid info")]
    private Vector2 gridSize;
    
    public void InitializeBlockInfo(int num)
    {
        blockNumber = num;
        blockClear = false;

        PolygonCollider2D collider = GetComponentInChildren<PolygonCollider2D>();
        min = collider.bounds.center - collider.bounds.size / 2;
        max = collider.bounds.center + collider.bounds.size / 2;
        blockSize = collider.bounds.size;
        blockPos = collider.bounds.center;

        gridSize = new Vector2(1.0f, 1.0f);
    }

    public Vector3 GridToWorldPosition(Vector2 gridPos)
    {
        Vector2 offset = gridPos * this.gridSize + (this.gridSize / 2);
        Vector3 pos = this.min + offset;
        if (IsInBlock(pos))
            return pos;
        else
        {
            Debug.LogWarning("SpawnPosition is not Block Domain");
            return (min + max)/2;
        }
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        Vector2 worldPos2d = new Vector2(worldPos.x, worldPos.y);
        Vector2 localPos = worldPos2d - min;
        Vector2Int gridPos = new Vector2Int((int)localPos.x / (int)gridSize.x, (int)localPos.y / (int)gridSize.y);

        return gridPos;
    }

    //매개변수 position이 현재 블록 내에 존재하는지 판단
    public bool IsInBlock(Vector3 pos)
    {
        if (pos.x > this.max.x || pos.x < this.min.x)
            return false;
        else if (pos.y > this.max.y || pos.y < this.min.y)
            return false;
        else
            return true;
    }

    public Vector2Int GetMaxGridPos()
    {
        return new Vector2Int((int)(blockSize.x / gridSize.x), (int)(blockSize.y / gridSize.y));
    }

    public Vector2 GetBlockCenter()
    {
        return (min + max) / 2;
    }

    public Vector2 GetBlockMin()
    {
        return min;
    }
    public Vector2 GetBlockMax()
    {
        return max;
    }

    public int CompareTo(object obj)
    {
        Vector2 thisPos = this.transform.position;
        Vector2 comparePos = (obj as BlockInfo).transform.position;

        if (thisPos.y > comparePos.y)
            return -1;
        else if (thisPos.y == comparePos.y)
        {
            if (thisPos.x < comparePos.x)
                return -1;
            return 1;
        }
        else
            return 1;
    }

}
