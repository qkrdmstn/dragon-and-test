using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnDB : IComparable
{
    public int blockNum;
    public int wave;
    public string monsterType;
    public Vector2Int spawnPosition;

    public SpawnDB(int blockNum, int wave, string monsterType, Vector2Int spawnPosition)
    {
        this.blockNum = blockNum;
        this.wave = wave;
        this.monsterType = monsterType;
        this.spawnPosition = spawnPosition;
    }

    public string StringSpawnDB()
    {
        string result = "blockNum: " + this.blockNum + " wave: " + this.wave + " monsterType: " + this.monsterType + " spawPos: " + this.spawnPosition;
        return result;
    }

    public int CompareTo(object obj)
    {
        SpawnDB other = (obj as SpawnDB);

        if (this.blockNum < other.blockNum)
            return -1;
        else if (this.blockNum == other.blockNum)
        {
            if (this.wave < other.wave)
                return -1;
            else if (this.wave == other.wave)
                return this.monsterType.CompareTo(other.monsterType);
            return 1;
        }
        return 1;

    }
}
