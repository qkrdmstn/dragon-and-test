using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNear : MonoBehaviour
{
    public int HP = 3;

    public float moveSpeed = 9.0f;

    public float recognitionRange = 100.0f;
    public float attackRange = 2.0f;
    public Chase chase;

    #region MonsterAttack
    public int damage = 1;
    public bool inAttack = false;
    public float cooldown = 3.0f;
    public float tempcool;
    public Collider2D[] collider2Ds;
    public Vector2 boxSize;
    public float armLength;
    #endregion

    #region Componets
    public Animator anim { get; private set; }
    public Rigidbody2D rigidBody { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public Collider2D col { get; private set; }
    #endregion

    #region States
    public MonsterStateMachineNear stateMachine { get; private set; }
    public MonsterChaseStateNear chaseState { get; private set; }
    public MonsterAttackStateNear attackState { get; private set; }
    #endregion

    public GameObject player;
    public Player playerScript;
    public GameObject eventManager;
    public Spawner spawn;
    public float tempSpeed;
    private Vector3 newpos;
    

    private void Awake()
    {
        stateMachine = new MonsterStateMachineNear();
        player = GameObject.FindWithTag("Player");
        eventManager = GameObject.FindWithTag("EditorOnly");
        //idleState = new MonsterIdleState(this, stateMachine, player);
        chaseState = new MonsterChaseStateNear(this, stateMachine, player);
        attackState = new MonsterAttackStateNear(this, stateMachine, player);
        tempSpeed = moveSpeed;
        GetComponent<Collider2D>().enabled = false;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        chase = GetComponent<Chase>();
        playerScript = player.GetComponent<Player>();
        spawn = eventManager.GetComponent<Spawner>();
        stateMachine.Initialize(chaseState);
    }

    private void Update()
    {
        stateMachine.currentState.Update();
    }

    public void OnDamaged(int damage)
    {
        HP -= damage;

        if (HP <= 0)
        {
            Destroy(gameObject);
            spawn.deathCount();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            OnDamaged(1);
        }
    }

    public void Attack()
    {
        inAttack = true;
        anim.SetTrigger("attacking");
    }

    public void hit()
    {
        GetComponent<Collider2D>().enabled = true;
        newpos = transform.position+(Vector3)(chase.tempDir * armLength);
        collider2Ds = Physics2D.OverlapBoxAll(newpos, boxSize, Mathf.Atan2(chase.tempDir.y, chase.tempDir.x) * Mathf.Rad2Deg);
        foreach(Collider2D collider in collider2Ds)
        {
            if(collider.CompareTag("Player"))
            {
                playerScript.OnDamamged(damage);
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    public void outAttack()
    {
        GetComponent<Collider2D>().enabled = false;
        inAttack = false;
    }


    private void OnDrawGizmos()
    {
        if(GetComponent<Collider2D>().enabled == true && newpos != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(newpos, boxSize);
        }
        
    }
}