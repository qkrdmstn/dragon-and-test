using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MonsterBase : MonoBehaviour
{
    #region Stats
    public int HP;
    public float playerMPGain;
    public int damage;
    #endregion

    #region Move
    public float moveSpeed;
    public float knockbackForce; // 넉백 힘
    public float knockbackDuration; // 넉백 지속 시간
    public bool isKnockedBack; // 넉백 상태 여부를 나타내는 변수
    public float knockbackTimer; // 넉백 지속 시간을 계산하는 타이머
    #endregion

    #region Componets
    public Animator anim { get; private set; }
    public Rigidbody2D rigidBody { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public Collider2D col { get; private set; }
    public GameObject player;
    public Player playerScript;
    public GameObject eventManager;
    public Spawner spawn;
    #endregion

    #region States
    public MonsterStateMachine stateMachine { get; private set; }
    #endregion


    [Header("CameraSetting")]
    public CamShakeProfile profile;
    public CinemachineImpulseSource impulseSource;

    public temp temp;


    public virtual void Awake()
    {
        stateMachine = new MonsterStateMachine();
        player = GameObject.FindWithTag("Player");
        eventManager = GameObject.FindObjectOfType <Spawner>().gameObject;
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
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public virtual void Update()
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0) isKnockedBack = false;
        }
    }

    //공격
    public virtual void Attack()
    {
        return;
    }

    //피격
    public void OnTriggerEnter2D(Collider2D collision)
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
    public void Knockback(Vector2 dir, float force)
    {
        if (!isKnockedBack)
        {
            isKnockedBack = true;
            knockbackTimer = knockbackDuration;

            rigidBody.velocity = Vector2.zero; // 현재 속도를 초기화
            rigidBody.AddForce(dir * force, ForceMode2D.Impulse); // 총알 방향으로 힘을 가함
        }
    }

    //데미지 처리
    public void OnDamaged(int damage)
    {
        //피격에 따른 카메라 진동
        //  CameraManager.instance.CameraShakeFromProfile(profile, impulseSource);

        HP -= damage;
        if (HP <= 0)
        {
            Dead();
        }
    }

    //죽음
    public void Dead()
    {
        playerScript.curMP = Mathf.Min(playerScript.maxMP, playerScript.curMP + playerMPGain);
        Destroy(gameObject);
        spawn.deathCount();
        temp.killScore += 1;
        //temp.instance.killScore += 1;
    }
}