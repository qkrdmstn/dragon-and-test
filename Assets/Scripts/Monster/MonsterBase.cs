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

public enum StatusEffects
{
    slow,
    sokbak,
    sturn
}

public class MonsterBase : MonoBehaviour
{
    [Header("Life info")]
    public int curHP = 10;
    public int maxHP = 10;

    [Header("Move info")]
    public float moveSpeed;
    private float tempMoveSpeed;

    [Header("Drop Items")]
    [Tooltip("Min(inclusive), Max(exclusive)")] public Vector2Int moneyRange;
    private GameObject[] dropItemPrefabs;
    private GameObject moneyPrefab;

    [Header("Monster Info")]
    public MonsterTypes monsterType;

    [Header("Anim Info")]
    public bool haveAnim = false;
    public float spawnDuration = 1.0f;
    [Range(0.0f, 0.7f)] public float deadDuration = 0.6f;

    [Header("BaseState Range Info")]
    public float chaseDist;
    public float attackDist;

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

    protected virtual void Awake()
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

        if(haveAnim)
            monsterAnimController = GetComponentInChildren<MonsterAnimController>();

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        dropItemPrefabs = Resources.LoadAll<GameObject>("Prefabs/Item/Item Obj - DragonFruit");
        moneyPrefab = Resources.Load<GameObject>("Prefabs/Item/Money");
    }

    protected virtual void Update()
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
        Debug.Log(player);
        Debug.Log(monsterAnimController);
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

    //넉백
    public virtual void Knockback(Vector2 dir, float vel)
    {
        //if (!isKnockedBack)
        //{
        //    isKnockedBack = true;
        //    knockbackTimer = 0.0f;
        //    dir.Normalize();
        //    knockbackVel = vel * dir;
        //    rigidBody.velocity = knockbackVel;
        //    //rigidBody.velocity = Vector2.zero; // 현재 속도를 초기화
        //    //rigidBody.AddForce(dir * force, ForceMode2D.Impulse); // 총알 방향으로 힘을 가함
        //}
    }

    public void DotDamage(float duration, float interval, int perDamage)
    {
        StartCoroutine(DotDamageCoroutine(duration, interval, perDamage));
    }

    IEnumerator DotDamageCoroutine(float duration, float interval, int perDamage)
    {
        float timer = interval;
        while (duration >= 0.0f)
        {
            yield return null;
            timer -= Time.deltaTime;
            duration -= Time.deltaTime;
            if (timer < 0.0f)
            {
                timer = interval;
                OnDamaged(perDamage);
                SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.Damage);
            }
        }
    }

    public void SetNormalSpeed()
    {
        agent.speed = tempMoveSpeed;
    }

    public void SetSlowSpeed(float scale)
    {
        if (agent.speed <= tempMoveSpeed - tempMoveSpeed * scale)
            return;
        tempMoveSpeed = agent.speed;
        agent.speed = moveSpeed - moveSpeed * scale;
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

    ////Temp OnDamaged Func
    //protected virtual void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Bullet"))
    //        OnDamaged(1);
    //}
}