using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject monsterA;
    public GameObject monsterB;
    public GameObject monsterC;
    public GameObject monsterElite;
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
    //public Vector3[] positions;

    //임시 구현
    public bool waveEnd = false;
    public int killCount = 0;
    public Transform[] points;

    void Start()
    {
        //areaCollider = possibleArea.GetComponent<Collider2D>();
        waveLeft = wave;
        List<GameObject> monsterList = new List<GameObject> {monsterA, monsterB, monsterC, monsterElite};
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

        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();
        newWave();
    }

    public void deathCount()
    {
        killCount++;
        monsterLeft--;

        if (monsterLeft==0 && !waveEnd)
        {
            newWave();
        }
    }

    void newWave()
    {
        waveLeft--;
        if (waveLeft < 0)
            waveEnd = true;

        positionDisplay = new GameObject[quantity];

        for (int i=0; i<quantity; i++)
        {
            positionDisplay[i] = Instantiate(prefabs, points[i].position, Quaternion.identity);
        }

        Invoke("SpawnMonster", 1.5f);
    }

    void SpawnMonster()
    {
        for (int i = 0; i < quantity; i++)
        {
            Debug.Log(spawnList[i]);
            Destroy(positionDisplay[i]);
            Instantiate(spawnList[i], points[i].position, Quaternion.identity);
            monsterLeft++;
        }
    }
}