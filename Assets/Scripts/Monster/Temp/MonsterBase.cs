using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public enum MonsterTypes
{
    normal,
    elite,
    boss
}

public class MonsterBase : MonoBehaviour
{
    [Header("Life info")]
    public int curHP = 10;
    public int maxHP = 10;

    [Header("Move info")]
    public float moveSpeed;

    [Header("Drop Items")]
    [Tooltip("Min(inclusive), Max(exclusive)")] public Vector2Int moneyRange;
    GameObject[] dropItemPrefabs;
    GameObject moneyPrefab;

    [Header("Monster Info")]
    public MonsterTypes monsterType;

    [Header("Anim Info")]
    public bool haveAnim = false;
    [Range(0.0f, 0.7f)] public float deadDuration = 0.6f;

    #region Other Components
    [HideInInspector] public Player player {  get; private set; }
    [HideInInspector] public Spawner spawner { get; private set; }
    #endregion

    #region Self Componets
    public Rigidbody2D rb { get; private set; }
    public MonsterAnimController monsterAnimController { get; private set; }
    private UnityEngine.AI.NavMeshAgent agent;
    #endregion

    #region States
    public MonsterStateMachine stateMachine;

    //모든 몬스터의 기본 States
    public MonsterSpawnStateBase spawnState;
    public MonsterIdleStateBase idleState;
    public MonsterChaseStateBase chaseState;
    public MonsterDeadStateBase deadState;
    #endregion

    private void Awake()
    {
        InitComponents();
        InitStates();
    }

    protected virtual void Start()
    {
        SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.Spawn);

        if (Player.instance.isTutorial) return;

        if (!IsEffectSpawner())
            return;

        spawner = GameObject.FindObjectOfType<Spawner>();
    }

    public virtual void InitStates()
    {
        stateMachine = new MonsterStateMachine();
    }

    public virtual void InitComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();

        monsterAnimController = GetComponentInChildren<MonsterAnimController>();

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        dropItemPrefabs = Resources.LoadAll<GameObject>("Prefabs/Item/Item Obj - DragonFruit");
        moneyPrefab = Resources.Load<GameObject>("Prefabs/Item/Money");
    }

    void Update()
    {
        stateMachine.currentState.Update();

    }

    public void SetSpeed(float speed)
    {
        if (speed == 0)
        {
            agent.speed = 0;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0;
        }
        else
            agent.speed = speed;
    }

    public void SetDestination(Vector3 dest)
    {
        agent.SetDestination(dest);
    }

    public Direction CheckDir()
    {
        Vector3 dir = player.transform.position - transform.position;
        return monsterAnimController.FindDirToPlayer(dir);
    }

    //데미지 처리
    public virtual void OnDamaged(int damage)
    {
        curHP -= damage;
        if (curHP <= 0)
        {
            stateMachine.ChangeState(deadState);
        }
        else SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.Damage);
    }

    //아이템 드랍
    public void ItemDrop()
    {
        HwatuObjectDrop();
        DropItems();
        MoneyDrop();
    }

    private void DropItems()
    {
        for (int i = 0; i < dropItemPrefabs.Length; i++)
        {
            ItemObject item = dropItemPrefabs[i].GetComponent<ItemObject>();
            float randomVal = Random.Range(0.0f, 1.0f);
            if (randomVal <= item.dropProb)
                Instantiate(dropItemPrefabs[i], this.transform.position, Quaternion.identity);
        }
    }

    public virtual void HwatuObjectDrop()
    {
        float randomVal = Random.Range(0.0f, 1.0f);
        if (randomVal <= 0.2f)
        {
            GameObject hwatuObj = Instantiate(SkillManager.instance.hwatuItemObj, this.transform.position, Quaternion.identity);
            int index = Random.Range(0, SkillManager.instance.hwatuData.Length);
            hwatuObj.GetComponent<HwatuItemObject>().hwatuData = SkillManager.instance.hwatuData[index];
            Debug.Log("Hwatu Drop");
        }
    }

    protected void MoneyDrop()
    {
        GameObject moneyObj = Instantiate(moneyPrefab, this.transform.position, Quaternion.identity);
        MoneyItemObject moneyItem = moneyObj.GetComponent<MoneyItemObject>();

        moneyItem.amount = Random.Range(moneyRange.x, moneyRange.y);

        SkillDB ttCatch73Data = SkillManager.instance.GetSkillDB(SeotdaHwatuCombination.TTCatch73);
        float ttCatch73Prob = SkillManager.instance.GetSkillProb(SeotdaHwatuCombination.TTCatch73);
        float randomVal = Random.Range(0.0f, 1.0f);
        if (SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.TTCatch73) && randomVal <= ttCatch73Prob)
        {
            if (this.name.Contains("BirdTanker") || this.name.Contains("BirdCrossbowman"))
                moneyItem.amount *= 2;
            moneyObj.transform.localScale = moneyObj.transform.lossyScale * 1.5f;
        }
    }

    public bool IsEffectSpawner()
    {
        if (ScenesManager.instance.GetSceneEnum() == SceneInfo.Boss_1 || SceneManager.GetActiveScene().name == "BossTest" || monsterType == MonsterTypes.boss)
            return false;
        else
            return true;
    }
}