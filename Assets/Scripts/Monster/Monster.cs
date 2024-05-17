using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Monster : MonoBehaviour
{
    #region Stats
    public int HP;
    public float playerMPGain;
    public int damage;
    #endregion

    #region Move
    public float moveSpeed;
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

    #region Navigate
    private UnityEngine.AI.NavMeshAgent agent;
    #endregion


    [Header("CameraSetting")]
    public CamShakeProfile profile;
    public CinemachineImpulseSource impulseSource;
   
    public virtual void Awake()
    {
        stateMachine = new MonsterStateMachine();
        player = GameObject.FindWithTag("Player");
        eventManager = GameObject.FindObjectOfType <Spawner>().gameObject;
    }

    public virtual void Start()
    {
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        playerScript = player.GetComponent<Player>();
        spawn = eventManager.GetComponent<Spawner>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        //navigate
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public virtual void Update()
    {
        agent.SetDestination(player.transform.position);
    }

    //공격
    public virtual void Attack()
    {
        return;
    }

    //피격
    public void Hit(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            OnDamaged(1);
        }
    }

    //데미지 처리
    public void OnDamaged(int damage)
    {
        // 피격에 따른 카메라 진동
        //CameraManager.instance.CameraShakeFromProfile(profile, impulseSource);

        HP -= damage;
        if (HP <= 0)
        {
            Dead();
        }
    }

    //죽음
    private void Dead()
    {
        Destroy(gameObject);
        temp.instance.killScore += 1;
    }
}