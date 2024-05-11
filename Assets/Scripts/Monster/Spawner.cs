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
    public int maxIteration = 50;
    private int waveLeft = 0;
    private int monsterLeft = 0;
    private GameObject[] spawnList;
    public GameObject prefabs;
    public GameObject possibleArea;
    public Collider2D areaCollider;
    public GameObject []positionDisplay;
    public Vector3[] positions;

    //임시 구현
    public bool waveEnd = false;
    public int killCount = 0;

    void Start()
    {
        areaCollider = possibleArea.GetComponent<Collider2D>();
        waveLeft = wave;
        List<GameObject> monsterList = new List<GameObject> {monsterA, monsterB, monsterC};
        /*
        spawnList = new GameObject[] { monsterList[2], monsterList[2],
                                        monsterList[1], monsterList[1], monsterList[1],
                                        monsterList[0],monsterList[0],monsterList[0],monsterList[0],monsterList[0]};
        */
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
        killCount++;
        monsterLeft--;
        if (killCount >= 10)
            waveEnd = true;
        if (monsterLeft==0 && !waveEnd)
        {
            newWave();
        }
    }

    void newWave()
    {
        positionDisplay = new GameObject[quantity];
        positions = new Vector3[quantity];
        int spawnQuantity = 0;
        for(int i=0; i<maxIteration; i++)
        {
            Vector3 spawnPosition = Random.insideUnitCircle * spawnRadius;
            spawnPosition += player.transform.position;
            if(areaCollider.OverlapPoint(spawnPosition))
            {
                positions[spawnQuantity] = spawnPosition;
                positionDisplay[spawnQuantity] = Instantiate(prefabs, spawnPosition, Quaternion.identity);
                spawnQuantity += 1;
            }
            if (spawnQuantity >= quantity) break;

            if (i==maxIteration-1) Debug.Log("Warning: space for monster spawn is too small. Monsters may not all spawn. Change maxIteration or spawnRadius");
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