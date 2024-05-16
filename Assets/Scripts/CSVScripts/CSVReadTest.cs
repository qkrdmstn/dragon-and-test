using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class CSVReadTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("SpawnDB/SpawnTabel1_ACSBV");

        for(int i=0; i<data.Count; i++)
        {
            Debug.Log("index " + (i).ToString() + data[i]["blockName"] + " " + data[i]["wave"] + " " + data[i]["monsterType"] + " " + data[i]["posX"] + " " + data[i]["posY"]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
