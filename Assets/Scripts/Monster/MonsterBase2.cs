using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MonsterBase2 : MonoBehaviour
{
    #region Stats
    [Header("Stats")]
    public int HP;
    public int damage;
    public float distanceToPlayer;
    public float deadSec = 0.6f;    // range : 0.0f ~ 0.7f
    [Tooltip("Min(inclusive), Max(exclusive)")] public Vector2Int moneyRange;
    #endregion

    #region Move
    [Header("Move & knockBack")]
    public float moveSpeed;
    public float tempMoveSpeed;
    public float knockbackForce; // 넉백 힘
    public float knockbackDuration; // 넉백 지속 시간
    public float knockbackTimer; // 넉백 지속 시간을 계산하는 타이머
    protected Vector2 knockbackVel;
    #endregion

    #region dropItems
    GameObject [] dropItems;
    GameObject money;
    #endregion

    #region Componets
    [HideInInspector] public Rigidbody2D rigidBody { get; protected set; }
    [HideInInspector] public SpriteRenderer spriteRenderer { get; private set; }
    [HideInInspector] public Collider2D col { get; protected set; }
    [HideInInspector] public GameObject player;
    [HideInInspector] public Player playerScript;
    [HideInInspector] public GameObject eventManager;
    [HideInInspector] public Spawner spawn;
    [HideInInspector] public MonsterAnimController monsterAnimController;
    #endregion

    #region States
    public MonsterStateMachine2 stateMachine { get; protected set; }
    public MonsterState2 tempState { get; protected set; }
    public MonsterEffectState effectState { get; protected set; }
    #endregion

    #region Navigate
    [HideInInspector] public UnityEngine.AI.NavMeshAgent agent;
    #endregion

    [Header("boolState")]
    public bool inEffect = false;
    public bool isDead = false;
    public bool isChase;
    public bool isKnockedBack;      // 넉백 상태 여부를 나타내는 변수

    // 애니메이션 작업이 완료된 몬스터에게만 적용되는 변수
    public bool isSpawned = false;  
    public bool isFirst = true;

    public virtual void Awake()
    {
        stateMachine = new MonsterStateMachine2();

        player = GameObject.FindWithTag("Player");

        dropItems = Resources.LoadAll<GameObject>("Prefabs/Item/Item Obj - DragonFruit");
        money = Resources.Load<GameObject>("Prefabs/Item/Money");

        //if (Player.instance.isTutorial) return;
        //if (ScenesManager.instance.GetSceneEnum() != SceneInfo.Boss_1 && SceneManager.GetActiveScene().name != "BossTest")
        //    eventManager = GameObject.FindObjectOfType<Spawner>().gameObject;
    }

    public virtual void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        playerScript = player.GetComponent<Player>();
        monsterAnimController = GetComponentInChildren<MonsterAnimController>();

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.Spawn);

        if (Player.instance.isTutorial) return;

        if(ScenesManager.instance.GetSceneEnum() != SceneInfo.Boss_1 && SceneManager.GetActiveScene().name != "BossTest")
        {
            eventManager = GameObject.FindObjectOfType<Spawner>().gameObject;
            spawn = eventManager.GetComponent<Spawner>();
        }
    }

    public virtual void Update()
    {
        if (isKnockedBack)
        {
            knockbackTimer += Time.deltaTime;
            //Exponantial
            knockbackVel = knockbackVel * Mathf.Exp(-0.1f * knockbackTimer);
            rigidBody.velocity = knockbackVel;
            if (rigidBody.velocity.magnitude <= 0.1f)
            {
                rigidBody.velocity = Vector2.zero;
                isKnockedBack = false;
            }
        }
    }

    //공격
    public virtual void Attack()
    {
        return;
    }

    //피격
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.CompareTag("Bullet"))
        //{
        //    PlayerNormalBullet bullet = collision.GetComponent<PlayerNormalBullet>();
        //    OnDamaged(bullet.damage);

        //    Vector2 dir = this.transform.position - player.transform.position;
        //    dir.Normalize();
        //    Knockback(dir, bullet.knockbackForce);
        //}
    }

    //넉백
    public virtual void Knockback(Vector2 dir, float vel)
    {
        if (!isKnockedBack)
        {
            isKnockedBack = true;
            knockbackTimer = 0.0f;
            dir.Normalize();
            knockbackVel = vel * dir;
            rigidBody.velocity = knockbackVel;
            //rigidBody.velocity = Vector2.zero; // 현재 속도를 초기화
            //rigidBody.AddForce(dir * force, ForceMode2D.Impulse); // 총알 방향으로 힘을 가함
        }
    }

    //데미지 처리
    public virtual void OnDamaged(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            Dead();
        }
        else SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.Damage);
    }

    public void DotDamage(float duration, float interval, int perDamage)
    {
        StartCoroutine(DotDamageCoroutine(duration, interval, perDamage));
    }

    IEnumerator DotDamageCoroutine(float duration, float interval, int perDamage)
    {
        float timer = interval;
        while(duration >= 0.0f)
        {
            yield return null;
            timer -= Time.deltaTime;
            duration -= Time.deltaTime;
            if (timer < 0.0f)
            {
                timer = interval;
                HP -= perDamage;
                SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.Damage);
            }
        }
    }

    //상태이상
    public void EffectState()
    {
        if (!inEffect) tempState = stateMachine.currentState;
        stateMachine.ChangeState(effectState);
    }

    //죽음
    public virtual void Dead()
    {
        if(!isDead)
        {
            isDead = true;
            SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.Dead);

            Destroy(gameObject);
            if (ScenesManager.instance.GetSceneEnum() != SceneInfo.Boss_1 && SceneManager.GetActiveScene().name != "BossTest")
            {
                spawn.DeathCount();
                ItemDrop();
            }
        }
    }

    protected virtual IEnumerator AnimSpawn()
    {
        SpeedToZero();
        monsterAnimController.SetAnim();
        yield return new WaitForSeconds(1f);

        isSpawned = true;
        SpeedReturn();
    }

    protected virtual IEnumerator AnimDead()
    {
        monsterAnimController.SetAnim(MonsterAnimState.Death, CheckDir());
        float sec = Mathf.Clamp(deadSec, 0f, 0.7f);
        yield return new WaitForSeconds(sec);

        if (ScenesManager.instance.GetSceneEnum() != SceneInfo.Boss_1 && SceneManager.GetActiveScene().name != "BossTest")
        {
            spawn.DeathCount();
            ItemDrop();
        }
        yield return new WaitForSeconds(0.7f - sec);

        Destroy(gameObject);
    }

    public virtual void SpeedToZero()
    {
        agent.speed = 0;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = 0;
    }

    public virtual void SpeedReturn()
    {
        agent.speed = moveSpeed;
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
        for (int i = 0; i < dropItems.Length; i++)
        {
            ItemObject item = dropItems[i].GetComponent<ItemObject>();
            float randomVal = Random.Range(0.0f, 1.0f);
            if (randomVal <= item.dropProb)
                Instantiate(dropItems[i], this.transform.position, Quaternion.identity);
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
        GameObject moneyObj = Instantiate(money, this.transform.position, Quaternion.identity);
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

    public Direction CheckDir()
    {
        Vector3 dir = player.transform.position - transform.position;
        return monsterAnimController.FindDirToPlayer(dir);
    }

    public void SetNormalSpeed()
    {
        Debug.Log("normal");
        agent.speed = tempMoveSpeed;
    }

    public void SetSlowSpeed(float scale)
    {
        Debug.Log("slow");
        if (agent.speed <= tempMoveSpeed - tempMoveSpeed * scale)
            return;
        tempMoveSpeed = agent.speed;
        agent.speed = moveSpeed - moveSpeed * scale;
    }
}