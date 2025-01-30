using System;
using System.Collections;
using System.Threading;
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
public struct MonsterStatusEffectsFlag
{
    public bool knockback; //넉백
    public bool slow; //슬로우
    public bool dotDamage; //도트
    public bool rooted; //속박
    public bool stun; //기절
    public bool reverse; //이동상태 변경

    public void InitStatusEffect()
    {
        knockback = false;
        slow = false;
        dotDamage = false;
        rooted = false;
        stun = false;
        reverse = false;
    }
}

public class MonsterBase : MonoBehaviour
{
    [Header("Life info")]
    public int curHP = 10;
    public int maxHP = 10;
    public bool isDead;

    [Header("Move info")]
    public float moveSpeed;
    private float tempMoveSpeed;

    [Header("Drop Items")]
    [Tooltip("Min(inclusive), Max(exclusive)")] public Vector2Int moneyRange;
    

    [Header("Monster Info")]
    public MonsterTypes monsterType;
    [SerializeField] public MonsterStatusEffectsFlag effectiveStatusEffects; //이 몬스터에게 효과가 있는 상태이상
    [SerializeField] public MonsterStatusEffectsFlag statusEffectsFlag; //몬스터가 영향 받는 중인 상태이상 flag

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

        statusEffectsFlag.InitStatusEffect();
    }

    protected virtual void Update()
    {
        stateMachine.currentState.Update();

        if(Input.GetKeyDown(KeyCode.H) && !statusEffectsFlag.rooted)
        {
            Rooted(0.1f);
        }

        if (Input.GetKeyDown(KeyCode.Y) && !statusEffectsFlag.stun)
        {
            Stun(0.2f);
        }
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
            isDead = true;
            stateMachine.ChangeState(deadState);
        }
        else SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.Damage);
    }

    #region Status Effect Func
    #region Knockback
    Coroutine knockbackVal;
    public void Knockback(Vector2 dir, float mag)
    {
        if (!effectiveStatusEffects.knockback || isDead)
            return;

        if (knockbackVal != null) //실행 중인 넉백 중단
            StopCoroutine(knockbackVal);

        knockbackVal = StartCoroutine(KnockbackCoroutine(dir, mag));
    }

    IEnumerator KnockbackCoroutine(Vector2 dir, float mag)
    {
        statusEffectsFlag.knockback = true;

        //넉백 도중 Idle State 고정
        if(!statusEffectsFlag.stun && !isDead) //이미 기절 상태라면 idle 상태로 변경할 필요 없음
        {
            isStateChangeable = true;
            stateMachine.ChangeState(idleState);
            isStateChangeable = false;
        }

        float knockbackTimer = 0.0f;
        dir.Normalize();

        while (true)
        {
            if(isDead)
            {
                statusEffectsFlag.knockback = false;
                isStateChangeable = true;
                stateMachine.ChangeState(deadState);
                break;
            }
            
            knockbackTimer += Time.deltaTime;
            yield return null;

            //Rigidbody based knockback
            //Exponantial
            mag = mag * Mathf.Exp(-0.1f * knockbackTimer);
            rb.velocity = mag * dir;
            if (rb.velocity.magnitude <= 0.1f)
            {
                rb.velocity = Vector2.zero;
                statusEffectsFlag.knockback = false;

                //state 고정 해제
                if (!statusEffectsFlag.stun || !isDead) //기절 or 사망 상태일 경우 state 변환 X
                {
                    isStateChangeable = true;
                    stateMachine.ChangeState(idleState);
                }
                break;
            }
        }
    }

    #endregion

    #region Slow

    //속도 변화 몬스터가 있는 경우 사용
    //public void SetSlowSpeed(float scale)
    //{
    //    if (!effectiveStatusEffects.slow)
    //        return;

    //    if ((agent.speed <= tempMoveSpeed - tempMoveSpeed * scale)) //이미 더 강력한 slow 효과를 받고 있을 경우
    //        return;

    //    if (!statusEffectsFlag.slow) //Slow 효과가 적용된 속도는 저장 X
    //        tempMoveSpeed = agent.speed; //원래 속도 저장

    //    agent.speed = moveSpeed - moveSpeed * scale; //slow 효과 적용
    //    statusEffectsFlag.slow = true;
    //}

    //public void SetNormalSpeed()
    //{
    //    if (!effectiveStatusEffects.slow)
    //        return;

    //    agent.speed = tempMoveSpeed; //속도 복구
    //    statusEffectsFlag.slow = false;
    //}

    //속도 변화 몬스터 X 경우 사용 (광전사는 면역)

    public void SetSlowSpeed(float scale)
    {
        if (!effectiveStatusEffects.slow)
            return;

        if ((agent.speed <= moveSpeed - moveSpeed * scale)) //이미 더 강력한 slow 효과를 받고 있을 경우
            return;

        SetSpeed(moveSpeed - moveSpeed * scale); //slow 효과 적용
        statusEffectsFlag.slow = true;
    }

    public void SetNormalSpeed()
    {
        if (!effectiveStatusEffects.slow)
            return;

        SetSpeed(moveSpeed); //속도 복구
        statusEffectsFlag.slow = false;
    }
    #endregion

    #region Dot Damage
    public void DotDamage(float duration, float interval, int perDamage)
    {
        if (!effectiveStatusEffects.dotDamage)
            return;

        StartCoroutine(DotDamageCoroutine(duration, interval, perDamage));
    }

    IEnumerator DotDamageCoroutine(float duration, float interval, int perDamage)
    {
        statusEffectsFlag.dotDamage = true;

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

        statusEffectsFlag.dotDamage = false;
    }
    #endregion

    #region Rooted
    Coroutine rootedVal;
    public void Rooted(float duration)
    {
        if (!effectiveStatusEffects.rooted || isDead)
            return;
        if (rootedVal != null) //실행 중인 속박 중단
            StopCoroutine(rootedVal);

        rootedVal = StartCoroutine(RootedCoroutine(duration));
    }

    IEnumerator RootedCoroutine(float duration)
    {
        statusEffectsFlag.rooted = true;
        SetSpeed(0);

        yield return new WaitForSeconds(duration);

        statusEffectsFlag.rooted = false;
        if(!statusEffectsFlag.slow && !statusEffectsFlag.stun)
            SetSpeed(moveSpeed);
    }
    #endregion

    #region Stun
    Coroutine stunVal;
    public void Stun(float duration)
    {
        if (!effectiveStatusEffects.stun || isDead)
            return;

        if (stunVal != null) //실행 중인 기절 중단
            StopCoroutine(stunVal);

        stunVal = StartCoroutine(StunCoroutine(duration));
    }

    IEnumerator StunCoroutine(float duration)
    {
        statusEffectsFlag.stun = true;
        if(!statusEffectsFlag.knockback && !isDead) //이미 넉백 중이라면 굳이 상태를 바꿀 필요 없음
        {
            stateMachine.ChangeState(idleState);
            isStateChangeable = false;
        }

        yield return new WaitForSeconds(duration);

        statusEffectsFlag.stun = false;
        if(!statusEffectsFlag.knockback)
            isStateChangeable = true;
    }
    #endregion

    #region Reverse
    Coroutine reverseVal;
    public void Reverse(float duration)
    {
        if (!effectiveStatusEffects.reverse || isDead)
            return;

        if (reverseVal != null) //실행 중인 reverse 중단
            StopCoroutine(reverseVal);

        reverseVal = StartCoroutine(ReverserCoroutine(duration));
    }

    public IEnumerator ReverserCoroutine(float duration)
    {
        statusEffectsFlag.reverse = true;

        yield return new WaitForSeconds(duration);

        statusEffectsFlag.reverse = false;
    }
    #endregion
    #endregion

    #region Drop Item
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

    #endregion

    public bool IsEffectSpawner()
    {
        if (ScenesManager.instance.GetSceneEnum() == SceneInfo.Boss_1 || SceneManager.GetActiveScene().name == "BossTest" || monsterType == MonsterTypes.boss)
            return false;
        else
            return true;
    }
}