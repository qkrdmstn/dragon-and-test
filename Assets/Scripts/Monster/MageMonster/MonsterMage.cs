using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMage : MonsterBase
{
    
    #region MonsterAttack
    public bool inAttack = false;
    private bool warning = false;
    public float cooldown = 8.0f;
    public float attackSpeed = 10.0f;
    public float tempcool;
    public Vector3 direction;
    private Vector3 endPosition;
    public SpriteRenderer spriteRenderer;
    public GameObject attackWarning;
    private Collider2D attackArea;
    #endregion

    #region States
    public MonsterIdleStateMage idleState { get; private set; }
    public MonsterChaseStateMage chaseState { get; private set; }
    public MonsterAttackStateMage attackState { get; private set; }
    public float chaseRange = 10.0f;
    public float attackRange = 4.0f;
    
    public float distanceToPlayer;
    #endregion

    #region Navigate
    public UnityEngine.AI.NavMeshAgent agent;
    #endregion

    public override void Awake()
    {
        base.Awake();
        idleState = new MonsterIdleStateMage(stateMachine, player, this);
        chaseState = new MonsterChaseStateMage(stateMachine, player, this);
        attackState = new MonsterAttackStateMage(stateMachine, player, this);
    }

    public override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);

        spriteRenderer = attackWarning.GetComponent<SpriteRenderer>();
        attackArea = attackWarning.GetComponent<Collider2D>();

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        SpeedToZero();
    }

    public override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        //Debug.Log(stateMachine.currentState);
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        tempcool -= Time.deltaTime;

        //navigate
        //agent.SetDestination(player.transform.position);

        //draw warning sign
        if (inAttack)
        {   
            if (warning)
            {
                endPosition = player.transform.position;
                spriteRenderer.transform.localScale = new Vector3(1f, direction.magnitude*1.35f, 1f);
                spriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
                spriteRenderer.transform.position = transform.position + (direction*1.35f)/2;
            }
            direction = endPosition - transform.position;
            direction.Normalize();
            direction *= 20f;
        }
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (inAttack && collision.gameObject.CompareTag("Player"))
        {
            playerScript.PlayerKnockBack(player.transform.position - transform.position, 10f); //Vector2 dir, float mag
        }
    }

    public override void Attack()
    {
        inAttack = true;
        warning = true;
        attackWarning.SetActive(true);
        
        Invoke("Shoot", 3f);
    }

    private void Shoot()
    {
        attackArea.enabled = true;

        Vector2 boxCenter = attackArea.bounds.center;
        Vector2 boxSize = attackArea.bounds.size;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log("player hit!");
                attackArea.enabled = false;
            }
        }

        attackArea.enabled = false;
        attackWarning.SetActive(false);
        inAttack = false;
    }

    public void SpeedToZero()
    {
        agent.speed = 0;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = 0.0f;
    }

    public void SpeedReturn()
    {
        agent.speed = moveSpeed;
    }
}