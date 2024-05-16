using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//public enum MonsterType1
//{
//    BirdWarrior,
//    BirdTanker,
//    BirdAcher,
//    BirdCrossbowman,
//    BirdBerserker
//}

public class Spawner : MonoBehaviour
{
    [Header("Spawn Table")]
    [SerializeField] private string fileName;
    [SerializeField] private int stage;
    List<Dictionary<string, object>> spawnTable;

    [Header("DB")]
    [SerializeField] private GameObject[] monsterPrefabs;
    [SerializeField] private GameObject circle; //몬스터 생성 위치
    [SerializeField] private Dictionary<string, GameObject> monsterDictionary;
    [SerializeField] private List<SpawnDB> spawnDB;

    [Header("Block info")]
    public BlockInfo[] blocks;
    public int curBlockNum;

    [Header("Wave info")]
    [SerializeField] private int curWave;
    [SerializeField] private int numMonster;

    [Header("Spawn info")]
    [SerializeField] private float spawnDelay = 1.5f;

    // Start is called before the first frame update
    void Awake()
    {
        InitializeMonsterDictionary();
        InitializeBlockInfo();
        InitializeSpawnDB();
    }

    public void Start()
    {
        UpdateCurBlockNumber(ComputeInitialBlockNumber());
    }

    #region Initialize Func
    private void InitializeBlockInfo()
    {
        blocks = FindObjectsOfType<BlockInfo>();
        Array.Sort(blocks);

        for (int i = 0; i < blocks.Length; i++)
            blocks[i].InitializeBlockInfo(i);
    }

    private void InitializeSpawnDB()
    {
        //CSV parsing
        spawnTable = CSVReader.Read("SpawnDB/" + fileName);
        spawnDB = new List<SpawnDB>();
        for (int i = 0; i < spawnTable.Count; i++)
        {
            int blockNum = int.Parse(spawnTable[i]["blockNumber"].ToString());
            int wave = int.Parse(spawnTable[i]["wave"].ToString());
            string monsterType = spawnTable[i]["monsterType"].ToString();
            float posX = float.Parse(spawnTable[i]["gridPosX"].ToString());
            float posY = float.Parse(spawnTable[i]["gridPosY"].ToString());
            Vector2 spawnPosition = new Vector2(posX, posY);

            SpawnDB data = new SpawnDB(blockNum, wave, monsterType, spawnPosition);
            spawnDB.Add(data);
        }
    }

    private void InitializeMonsterDictionary()
    {
        //Monster Dictionary Setting
        monsterPrefabs = Resources.LoadAll<GameObject>("Prefabs/Monster/Stage" + stage.ToString());
        monsterDictionary = new Dictionary<string, GameObject>();
        for (int i = 0; i < monsterPrefabs.Length; i++)
        {
            monsterDictionary.Add(monsterPrefabs[i].name, monsterPrefabs[i]);
        }
    }
    #endregion

    private int ComputeInitialBlockNumber()
    {
        //플레이어와 가장 가까운 block이 시작 block
        Vector3 playerPos = GameManager.instance.player.transform.position; //GameManager.instance.player 초기화는 Awake 이후
        int idx = 0;
        float minDist = Vector3.Magnitude(blocks[0].transform.position - playerPos);
        for (int i = 1; i < blocks.Length; i++)
        {
            float dist = Vector3.Magnitude(blocks[i].transform.position - playerPos);
            if (dist < minDist)
            {
                minDist = dist;
                idx = i;
            }
        }

        curWave = 0;
        return blocks[idx].blockNumber;
    }

    public void UpdateCurBlockNumber(int blockNum)
    {
        curBlockNum = blockNum;
        if (!blocks[curBlockNum].blockClear)
        {
            //Wave Start
            newWave();
        }
    }

    private void newWave()
    {
        //Setting Wave Data
        numMonster = 0; //wave monster count
        int idx = 0;
        while (spawnDB[idx].blockNum != curBlockNum || spawnDB[idx].wave != curWave)
        {
            idx++;
            if(idx >= spawnDB.Count) //끝까지 일치하는 정보를 못 찾은 경우
            {
                BlockClear();
                return;
            }
        }
        
        List<GameObject> monsters = new List<GameObject>();
        List<Vector3> positions = new List<Vector3>();
        while (spawnDB[idx].blockNum == curBlockNum && spawnDB[idx].wave == curWave)
        {
            GameObject monster = monsterDictionary[spawnDB[idx].monsterType];
            //GridPos -> WorldPos
            Vector3 worldPos = blocks[curBlockNum].GridToWorldPosition(spawnDB[idx].spawnGridPos);
            monsters.Add(monster);
            positions.Add(worldPos);
            idx++;
            numMonster++;

            if (idx >= spawnDB.Count) //끝까지 찾음
                break;
        }       
        StartCoroutine(SpawnMonster(monsters, positions));
    }

    IEnumerator SpawnMonster(List<GameObject> monsters, List<Vector3> positions)
    {
        //위치 미리보기
        List<GameObject> displayPos = new List<GameObject>();
        for(int i=0; i< positions.Count; i++)
            displayPos.Add(Instantiate(circle, positions[i], Quaternion.identity));

        //Spawn Delay
        float spawnTimer = spawnDelay;
        while(spawnTimer >= 0.0)
        {
            spawnTimer -= Time.deltaTime;
            yield return null;
        }

        //몬스터 생성
        for (int i = 0; i < positions.Count; i++)
        {
            Destroy(displayPos[i]);
            Instantiate(monsters[i], positions[i], Quaternion.identity);
        }
    }

    public void DeathCount()
    {
        numMonster--;
        //Wave End
        if (numMonster <= 0)
            WaveClear();
    }

    private void WaveClear()
    {
        curWave++;
        newWave();
    }

    private void BlockClear()
    {
        curWave = 0;
        blocks[curBlockNum].blockClear = true;
    }
}