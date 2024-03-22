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
    public int quantity = 10;
    private int waveLeft = 0;
    private int monsterLeft = 0;
    private GameObject[] spawnList;
    public GameObject prefabs;

    public GameObject []positionDisplay;
    public Vector3[] positions;
    void Start()
    {
        waveLeft = wave;
        List<GameObject> monsterList = new List<GameObject> {monsterA, monsterB, monsterC};
        spawnList = new GameObject[] { monsterList[2], monsterList[2],
                                        monsterList[1], monsterList[1], monsterList[1],
                                        monsterList[0],monsterList[0],monsterList[0],monsterList[0],monsterList[0]};
        //for (int i=0;i<quantity;i++)
        //{
        //    if (i<monsterList.Count) spawnList[i] = monsterList[i];
        //    else spawnList[i] = monsterList[Random.Range(0, monsterList.Count)];
        //}
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
        positionDisplay = new GameObject[quantity];
        positions = new Vector3[quantity]; 
        for(int i=0; i<quantity; i++)
        {
            Vector3 spawnPosition = Random.insideUnitCircle * spawnRadius;
            spawnPosition += player.transform.position;
            positions[i] = spawnPosition;
            positionDisplay[i] = Instantiate(prefabs, spawnPosition, Quaternion.identity);
        }

        Invoke("SpawnMonster", 2.0f);

    }

    void SpawnMonster()
    {
        for (int i = 0; i < quantity; i++)
        {
            Destroy(positionDisplay[i]);
            Instantiate(spawnList[i], positions[i], Quaternion.identity);
            monsterLeft++;
        }
    }
}