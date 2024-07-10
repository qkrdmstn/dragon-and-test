using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MonsterBase : MonoBehaviour
{
    #region Stats
    public int HP;
    public int damage;
    #endregion

    #region Move
    public float moveSpeed;
    public float knockbackForce; // 넉백 힘
    public float knockbackDuration; // 넉백 지속 시간
    public bool isKnockedBack; // 넉백 상태 여부를 나타내는 변수
    public float knockbackTimer; // 넉백 지속 시간을 계산하는 타이머
    public Vector2 knockbackVel;
    #endregion

    #region Drop Items
    public GameObject[] dropItems;
    #endregion
    #region Componets
    public Animator anim { get; protected set; }
    public Rigidbody2D rigidBody { get; protected set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public Collider2D col { get; protected set; }
    public GameObject player;
    public Player playerScript;
    public GameObject eventManager;
    public Spawner spawn;
    #endregion

    #region States
    public MonsterStateMachine stateMachine { get; protected set; }
    public MonsterEffectState effectState { get; protected set; }
    public MonsterState tempState { get; protected set; }
    #endregion

    public temp temp;
    public bool inEffect = false;
    public bool isDead = false;


    public virtual void Awake()
    {
        stateMachine = new MonsterStateMachine();
        effectState = new MonsterEffectState(stateMachine, player, this);

        player = GameObject.FindWithTag("Player");
        eventManager = GameObject.FindObjectOfType<Spawner>().gameObject;

        dropItems = Resources.LoadAll<GameObject>("Prefabs/Item");
    }

    public virtual void Start()
    {
        temp = GameObject.FindObjectOfType<temp>();
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        playerScript = player.GetComponent<Player>();
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
            OnDamaged(1);

            Vector2 dir = this.transform.position - player.transform.position;
            dir.Normalize();
            Knockback(dir, knockbackForce);
        }
    }

    //넉백
    public void Knockback(Vector2 dir, float vel)
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
        if (HP <= 0)
        {
            Dead();
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
            spawn.DeathCount();
            temp.killScore += 1;
            ItemDrop();
            //temp.instance.killScore += 1;
        }
    }

    public void ItemDrop()
    {
        int index = Random.Range(0, dropItems.Length);
        Instantiate(dropItems[index], this.transform.position, Quaternion.identity);
    }
}