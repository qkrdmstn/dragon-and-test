using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject monsterA;
    public GameObject monsterB;
    public GameObject monsterC;
    public GameObject player;
    public float spawnRadius = 5f;
    public int wave = 3;
    public int quantity = 5;
    private int waveLeft = 0;
    private int monsterLeft = 0;
    private GameObject[] spawnList;

    void Start()
    {
        waveLeft = wave;
        List<GameObject> monsterList = new List<GameObject> {monsterA, monsterB, monsterC};
        spawnList = new GameObject[quantity];
        for (int i=0;i<quantity;i++)
        {
            if (i<monsterList.Count) spawnList[i] = monsterList[i];
            else spawnList[i] = monsterList[Random.Range(0, monsterList.Count)];
        }
        
        newWave();
    }

    public void deathCount()
    {
        monsterLeft--;
        if (monsterLeft==0)
        {
            newWave();
        }
    }

    void newWave()
    {
        for (int i=0; i<quantity; i++)
        {
            Vector3 spawnPosition = Random.insideUnitCircle * spawnRadius;
            spawnPosition += player.transform.position;

            Instantiate(spawnList[i], spawnPosition, Quaternion.identity);
            monsterLeft++;
        }

    }
}