using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnDB
{
    public int blockNum;
    public int wave;
    public string monsterType;
    public Vector2 spawnGridPos;

    public SpawnDB(int blockNum, int wave, string monsterType, Vector2 spawnPosition)
    {
        this.blockNum = blockNum;
        this.wave = wave;
        this.monsterType = monsterType;
        this.spawnGridPos = spawnPosition;
    }

    public string StringSpawnDB()
    {
        string result = "blockNum: " + this.blockNum + " wave: " + this.wave + " monsterType: " + this.monsterType + " spawPos: " + this.spawnGridPos;
        return result;
    }
}
