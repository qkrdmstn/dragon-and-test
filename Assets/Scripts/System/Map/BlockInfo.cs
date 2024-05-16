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

        PolygonCollider2D collider = this.GetComponentInChildren<PolygonCollider2D>();
        blockSize = collider.bounds.size;
        min =  collider.bounds.center - collider.bounds.size / 2;
        max =  collider.bounds.center + collider.bounds.size / 2;
        blockPos = min;

        gridSize = new Vector2(1.0f, 1.0f);
    }

    public Vector3 GridToWorldPosition(Vector2 gridPos)
    {
        Vector2 offset = gridPos * this.gridSize + (this.gridSize / 2);
        Vector3 pos = this.blockPos + offset;
        if (IsInBlock(pos))
            return pos;
        else
        {
            Debug.LogWarning("SpawnPosition is not Block Domain");
            return min;
        }
    }

    private bool IsInBlock(Vector3 pos)
    {
        if (pos.x > this.max.x || pos.x < this.min.x)
            return false;
        else if (pos.y > this.max.y || pos.y < this.min.y)
            return false;
        else
            return true;
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
