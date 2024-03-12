using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject monsterSpawn;
    public GameObject player;
    public float spawnRadius = 5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector3 spawnPosition = Random.insideUnitCircle * spawnRadius;
            spawnPosition += player.transform.position;

            Instantiate(monsterSpawn, spawnPosition, Quaternion.identity);
        }
    }
}