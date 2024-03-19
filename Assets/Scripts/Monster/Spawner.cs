using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject monsterSpawn;
    public GameObject player;
    public float spawnRadius = 5f;
    public int wave = 3;
    public int quantity = 5;
    private int waveLeft = 0;
    private int monsterLeft = 0;
    
    void Start()
    {
        waveLeft = wave;
        newWave();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
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

            Instantiate(monsterSpawn, spawnPosition, Quaternion.identity);
            monsterLeft++;
        }

    }
}