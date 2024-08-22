using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Table")]
    [SerializeField] private int stage;
    public SheetType myType;

    private delegate void ControlSpawnData();
    ControlSpawnData controlSpawnData;

    [Header("DB")]
    [SerializeField] private GameObject[] monsterPrefabs;
    [SerializeField] private GameObject circle; //몬스터 생성 위치
    [SerializeField] private Dictionary<string, GameObject> monsterDictionary;
    [SerializeField] private SpawnDB[] spawnDB;

    [Header("Block info")]
    public List<BlockInfo> blocks;
    public int curBlockNum;
    public Vector2[] mapHierarchy;
    //MapIndicator mapIndicator;

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

        controlSpawnData = StartWave;
    }

    public async void Start()
    {
        await LoadSpawnTable(myType); // load -> data slpit -> wave start 
    }

    #region Initialize Func
    private void InitializeBlockInfo()
    {
        //mapIndicator = FindObjectOfType<MapIndicator>();
        int layer = 1 << LayerMask.NameToLayer("MapSort");
        for(int i=0; i<mapHierarchy.Length; i++)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(mapHierarchy[i], Vector2.right, 250f, layer);
            foreach(RaycastHit2D hit in hits)
            {
                blocks.Add(hit.transform.GetComponentInParent<BlockInfo>());
            }
        }
        blocks = blocks.Distinct().ToList();
        //blocks = FindObjectsOfType<BlockInfo>();
        //Array.Sort(blocks);

        for (int i = 0; i < blocks.Count; i++)
            blocks[i].InitializeBlockInfo(i);
    }

    public async Task LoadSpawnTable(SheetType _type)
    {
        spawnDB = await DataManager.instance.GetValues<SpawnDB>(_type,"A1:E");
        controlSpawnData();

        ScenesManager.instance.isLoadedDB++;

    }

    void StartWave()
    {
        UpdateCurBlockNumber(ComputeInitialBlockNumber());
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
        Vector3 playerPos = Player.instance.transform.position; //GameManager.instance.player 초기화는 Awake 이후
        int idx = 0;
        float minDist = Vector3.Magnitude(blocks[0].transform.position - playerPos);
        for (int i = 1; i < blocks.Count; i++)
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
        if (blocks[curBlockNum].blockClear)
            return;

        //Setting Wave Data
        numMonster = 0; //wave monster count
        int idx = 0;
        while (spawnDB[idx].blockNum != curBlockNum || spawnDB[idx].wave != curWave)
        {
            idx++;
            if(idx >= spawnDB.Length) //끝까지 일치하는 정보를 못 찾은 경우
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
            Vector3 worldPos = blocks[curBlockNum].GridToWorldPosition(spawnDB[idx].TransIntToVector());
            monsters.Add(monster);
            positions.Add(worldPos);
            idx++;
            numMonster++;

            if (idx >= spawnDB.Length) //끝까지 찾음
                break;
        }
        //StartCoroutine(SpawnMonster(monsters, positions));
        SpawnMonster(monsters, positions);
    }

    void SpawnMonster(List<GameObject> monsters, List<Vector3> positions)
    {
        //몬스터 생성
        for (int i = 0; i < positions.Count; i++)
        {
            Instantiate(monsters[i], positions[i], Quaternion.identity);
        }
    }

    //IEnumerator SpawnMonster(List<GameObject> monsters, List<Vector3> positions)
    //{
    //    //위치 미리보기
    //    List<GameObject> displayPos = new List<GameObject>();
    //    for(int i=0; i< positions.Count; i++)
    //        displayPos.Add(Instantiate(circle, positions[i], Quaternion.identity));

    //    //Spawn Delay
    //    yield return new WaitForSeconds(spawnDelay);

    //    //몬스터 생성
    //    for (int i = 0; i < positions.Count; i++)
    //    {
    //        Destroy(displayPos[i]);
    //       Instantiate(monsters[i], positions[i], Quaternion.identity);
    //    }
    //}

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
        blocks[curBlockNum].blockClear = true;
        curWave = 0;
        //mapIndicator.BlockClear(curBlockNum);
    }
}