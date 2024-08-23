using System.Collections;
using UnityEngine;

public class MonsterBase : MonoBehaviour
{
    #region Stats
    [Header("Stats")]
    public int HP;
    public int damage;
    public float distanceToPlayer;
    [Tooltip("Min(inclusive), Max(exclusive)")] public Vector2Int moneyRange;
    #endregion

    #region Move
    [Header("Move & knockBack")]
    public float moveSpeed;
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
    public MonsterStateMachine stateMachine { get; protected set; }
    public MonsterEffectState effectState { get; protected set; }
    public MonsterState tempState { get; protected set; }
    #endregion

    #region Navigate
    [HideInInspector] public UnityEngine.AI.NavMeshAgent agent;
    #endregion

    [Header("boolState")]
    public bool inEffect = false;
    public bool isDead = false;
    public bool isChase;
    public bool isKnockedBack; // 넉백 상태 여부를 나타내는 변수

    public virtual void Awake()
    {
        stateMachine = new MonsterStateMachine();
        effectState = new MonsterEffectState(stateMachine, player, this);

        player = GameObject.FindWithTag("Player");

        dropItems = Resources.LoadAll<GameObject>("Prefabs/Item/Item Obj - DragonFruit");
        money = Resources.Load<GameObject>("Prefabs/Item/Money");

        if (Player.instance.isTutorial) return;

        if (ScenesManager.instance.GetSceneEnum() != SceneInfo.Boss_1)
            eventManager = GameObject.FindObjectOfType<Spawner>().gameObject;
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

        if (Player.instance.isTutorial) return;

        if(ScenesManager.instance.GetSceneEnum() != SceneInfo.Boss_1)
            spawn = eventManager.GetComponent<Spawner>();
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
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = collision.GetComponent<Bullet>();
            OnDamaged(bullet.damage);

            Vector2 dir = this.transform.position - player.transform.position;
            dir.Normalize();
            Knockback(dir, bullet.knockbackForce);
        }
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
        SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.Damage);

        HP -= damage;
        //피격 시, 패시브 스킬
        //독사
        if(SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.DS14))
        {
            //지속 데미지 -> duration과 interval이 없음
            StartCoroutine(DOTDamage(5.0f, 1.0f, 1));
        }
        //구삥
        if(SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.GPP19))
        {
            //9% 확률로 몬스터 기절 duration
        }

        if (HP <= 0)
        {
            Dead();
        }
    }

    IEnumerator DOTDamage(float duration, float interval, int perDamage)
    {
        float timer = interval;
        while(duration < 0.0f)
        {
            yield return null;
            timer -= Time.deltaTime;

            if(timer < 0.0f)
            {
                duration -= interval;
                timer = interval;
                HP -= perDamage;
            }
        }
    }

    //상태이상
    public void EffectState()
    {
        if (!inEffect) tempState = stateMachine.currentState;
        //Debug.Log(tempState);
        stateMachine.ChangeState(effectState);
    }

    //죽음
    public virtual void Dead()
    {
        if(!isDead)
        {
            isDead = true;
            Destroy(gameObject);
            if (ScenesManager.instance.GetSceneEnum() != SceneInfo.Boss_1)
            {
                spawn.DeathCount();
                ItemDrop();
            }

        }
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
        if (randomVal <= 0.1f)
        {
            GameObject hwatuObj = Instantiate(SkillManager.instance.hwatuItemObj, this.transform.position, Quaternion.identity);
            int index = Random.Range(0, SkillManager.instance.hwatuData.Length);
            hwatuObj.GetComponent<HwatuItemObject>().hwatuData = SkillManager.instance.hwatuData[index];
            Debug.Log("Hwatu Drop");
        }
    }

    protected void MoneyDrop()
    {
        ItemObject item = money.GetComponent<ItemObject>();
        float randomVal = Random.Range(0.0f, 1.0f);
        if (randomVal <= item.dropProb)
        {
            GameObject moneyObj = Instantiate(money, this.transform.position, Quaternion.identity);
            moneyObj.GetComponent<InteractionData>().sequence = Random.Range(moneyRange.x, moneyRange.y);
        }
    }

    public Direction CheckDir()
    {
        Vector3 dir = player.transform.position - transform.position;
        return monsterAnimController.FindDirToPlayer(dir);
    }
}