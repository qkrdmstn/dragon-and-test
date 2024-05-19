using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnEditor_BlockInfo : MonoBehaviour, IComparable
{
    public int blockNumber;
    public Vector2 blockCenterPos;

    [Header("Collider info")]
    public Vector2 min;
    public Vector2 max;
    public Vector2 blockSize;

    [Header("Grid info")]
    public Vector2 gridSize;

    [Header("Spawn info")]
    public List<SpawnDB> blockSpawnData;
    public List<GameObject> monsterVisualizeList;
    public int maxWave;

    #region Initialize
    public void InitializeBlockInfo(int num)
    {
        blockNumber = num;

        PolygonCollider2D collider = this.GetComponentInChildren<PolygonCollider2D>();
        blockSize = collider.bounds.size;
        min = collider.bounds.center - collider.bounds.size / 2;
        max = collider.bounds.center + collider.bounds.size / 2;
        blockCenterPos = (min + max) / 2;

        gridSize = new Vector2(1.0f, 1.0f);

        blockSpawnData = new List<SpawnDB>();
        monsterVisualizeList = new List<GameObject>();
        maxWave = 0;
    }

    public void InitializeBlockInfo(Vector2 _min, Vector2 _max)
    {
        min = _min;
        max = _max;
        blockCenterPos = (min + max) / 2;

        gridSize = new Vector2(1.0f, 1.0f);
    }
    #endregion

    #region Spawn Data Manage
    //Data Manage
    public void AddSpawnData(int wave, string monsterType, Vector2Int gridPos)
    {
        SpawnDB spawnDB = new SpawnDB(this.blockNumber, wave, monsterType, gridPos);

        for (int i = 0; i < blockSpawnData.Count; i++) //같은 wave에 같은 셀 중복 방지
        {
            if(blockSpawnData[i].wave == wave && blockSpawnData[i].spawnGridPos == gridPos)
            {
                Debug.LogWarning("A monster already exists in that space.");
                return;
            }
        }

        blockSpawnData.Add(spawnDB);
        SortingSpawnData();
        VisualizeSpawnData();
    }

    public void DeleteSpawnData(int wave, Vector2 gridPos)
    {
        if(SpawnEditorManager.instance.curWave == maxWave || IsContinuousWave(wave, gridPos))
        {
            for (int i = 0; i < blockSpawnData.Count; i++)
            {
                if (blockSpawnData[i].wave == wave && blockSpawnData[i].spawnGridPos == gridPos)
                {
                    blockSpawnData.Remove(blockSpawnData[i]);
                }
            }
        }
        else
        {
            Debug.LogWarning("Deleting this monster will create an empty wave.");
            return;
        }
        SortingSpawnData();
        VisualizeSpawnData();
    }

    public void SortingSpawnData()
    {
        blockSpawnData.Sort(delegate (SpawnDB x, SpawnDB y)
        {
            return x.CompareTo(y);
        });
        if (blockSpawnData.Count > 0)
            maxWave = blockSpawnData[blockSpawnData.Count - 1].wave;
        else
            maxWave = 0;
    }

    public bool IsContinuousWave(int curWave)
    {
        int wave = 0;
        for(int i=0; i<=curWave; i++)
        {
            for (int j = 0; j < blockSpawnData.Count; j++)
            {
                if (blockSpawnData[j].wave == wave)
                {
                    wave++;
                    break;
                }
            }
        }

        if (curWave == wave - 1)
            return true;
        else
            return false;

    }

    public bool IsContinuousWave(int deleteWave, Vector2 deleteGridPos) //삭제를 가정하고 빈 wave check
    {
        int wave = 0;
        for (int i = 0; i <= maxWave; i++)
        {
            for (int j = 0; j < blockSpawnData.Count; j++)
            {
                if (blockSpawnData[j].wave == deleteWave && blockSpawnData[j].spawnGridPos == deleteGridPos)
                    continue;
                if (blockSpawnData[j].wave == wave)
                {
                    wave++;
                    break;
                }
            }
        }
        if (maxWave == wave - 1)
            return true;
        else
            return false;

    }
    #endregion

    #region SpawnData Visualize
    public void VisualizeSpawnData()
    {
        ClearVisualSpawnData();
        for (int i = 0; i < blockSpawnData.Count; i++)
        {
            if(SpawnEditorManager.instance.curWave == blockSpawnData[i].wave)
            {
                GameObject[] monsterObj = SpawnEditorManager.instance.monsterObj;
                for (int j = 0; j < monsterObj.Length; j++)
                {
                    if (blockSpawnData[i].monsterType == monsterObj[j].name)
                    {
                        //GridPos -> WorldPos
                        Vector3 worldPos = GridToWorldPosition(blockSpawnData[i].spawnGridPos);
                        monsterVisualizeList.Add(Instantiate(monsterObj[j], worldPos, Quaternion.identity));
                        break;
                    }
                }
            }
        }
    }

    public void ClearVisualSpawnData()
    {
        for(int i=0; i< monsterVisualizeList.Count; i++)
        {
            Destroy(monsterVisualizeList[i]);
        }
        monsterVisualizeList.Clear();
    }
    #endregion

    #region Position Transform
    public Vector3 GridToWorldPosition(Vector2 gridPos)
    {
        Vector2 offset = gridPos * this.gridSize + (this.gridSize / 2);
        Vector3 pos = this.min + offset;
        if (IsInBlock(pos))
            return pos;
        else
        {
            Debug.LogWarning("SpawnPosition is not Block Domain");
            return blockCenterPos;
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
    #endregion

    public int CompareTo(object obj)
    {
        Vector2 thisPos = this.transform.position;
        Vector2 comparePos = (obj as SpawnEditor_BlockInfo).transform.position;

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
