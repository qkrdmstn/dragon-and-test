using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;
using Random = UnityEngine.Random;

public enum MonsterTypes
{
    normal,
    elite,
    boss
}

[Serializable]
public struct MonsterStatusEffectsCheck
{
    public bool isKnockback; //넉백
    public bool isSlowed; //슬로우
    public bool isDotDamaged; //도트
    public bool isRooted; //속박
    public bool isStun; //기절

    public void InitStatusEffect()
    {
        isKnockback = false;
        isSlowed = false;
        isDotDamaged = false;
        isRooted = false;
        isStun = false;
    }
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
    

    [Header("Monster Info")]
    public MonsterTypes monsterType;
    [SerializeField] public MonsterStatusEffectsCheck statusEffectsCheck;

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

    public bool isStateChangeable; //state 변경 방지

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
        stateMachine = new MonsterStateMachine(this);
        isStateChangeable = true;
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

        statusEffectsCheck.InitStatusEffect();

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
    Coroutine knockbackVal;
    public virtual void Knockback(Vector2 dir, float mag)
    {
        if (knockbackVal != null)
            StopCoroutine(knockbackVal);

        knockbackVal = StartCoroutine(KnockbackCoroutine(dir, mag));
    }

    IEnumerator KnockbackCoroutine(Vector2 dir, float mag)
    {
        statusEffectsCheck.isKnockback = true;

        //넉백 도중 Idle State 고정
        isStateChangeable = true;
        stateMachine.ChangeState(idleState);
        isStateChangeable = false;

        float knockbackTimer = 0.0f;
        dir.Normalize();

        while (true)
        {
            knockbackTimer += Time.deltaTime;
            yield return null;

            //Rigidbody based knockback
            //Exponantial
            mag = mag * Mathf.Exp(-0.5f * knockbackTimer);
            rb.velocity = mag * dir;
            if (rb.velocity.magnitude <= 0.1f)
            {
                rb.velocity = Vector2.zero;
                statusEffectsCheck.isKnockback = false;

                //state 고정 해제
                isStateChangeable = true;
                stateMachine.ChangeState(idleState);
                break;
            }
        }
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
        agent.speed = tempMoveSpeed; //속도 복구
        statusEffectsCheck.isSlowed = false;
    }

    public void SetSlowSpeed(float scale)
    {
        if ((agent.speed <= tempMoveSpeed - tempMoveSpeed * scale)) //이미 더 강력한 slow 효과를 받고 있을 경우
            return;

        if (!statusEffectsCheck.isSlowed) //Slow 효과가 적용된 속도는 저장 X
            tempMoveSpeed = agent.speed; //원래 속도 저장

        agent.speed = moveSpeed - moveSpeed * scale; //slow 효과 적용
        statusEffectsCheck.isSlowed = true;
    }

    //아이템 드랍
    public void ItemDrop()
    {
        HwatuObjectDrop();
        DropItems();
        MoneyDrop();
    }

    private void DropItems()
    {   // 현재 용과만 드랍되므로 반복문 삭제
        float randomVal = Random.Range(0.0f, 1.0f);
        ItemObject item = ItemManager.instance.fruitPrefab.GetComponent<ItemObject>();
        if (randomVal <= item.dropProb)
            Instantiate(item.gameObject, this.transform.position, Quaternion.identity);
    }

    public virtual void HwatuObjectDrop()
    {
        float randomVal = Random.Range(0.0f, 1.0f);
        if (randomVal <= 0.2f)
        {
            GameObject hwatuObj = Instantiate(ItemManager.instance.hwatuItemObj, this.transform.position, Quaternion.identity);
            int index = Random.Range(0, ItemManager.instance.hwatuDatas.Length);
            hwatuObj.GetComponent<HwatuItemObject>().hwatuData = ItemManager.instance.hwatuDatas[index];
            Debug.Log("Hwatu Drop");
        }
    }

    protected void MoneyDrop()
    {
        GameObject moneyObj = Instantiate(ItemManager.instance.moneyPrefab, this.transform.position, Quaternion.identity);
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