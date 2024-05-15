using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNear : MonsterBase
{
    
    #region MonsterAttack
    public bool inAttack = false;
    public float cooldown = 3.0f;
    public float tempcool;
    public Collider2D[] collider2Ds;
    public Vector2 boxSize;
    public float armLength;
    private Vector3 newpos;
    public LayerMask playerMask;  
    #endregion

    #region States
    public MonsterChaseStateNear chaseState { get; private set; }
    public MonsterAttackStateNear attackState { get; private set; }
    public float attackRange = 1.0f;
    #endregion

    #region Navigate
    public UnityEngine.AI.NavMeshAgent agent;
    #endregion

    public float distanceToPlayer;
    public Vector3 direction;

    public override void Awake()
    {
        base.Awake();
        chaseState = new MonsterChaseStateNear(stateMachine, player, this);
        attackState = new MonsterAttackStateNear(stateMachine, player, this);
    }

    public override void Start()
    {
        base.Start();

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        stateMachine.Initialize(chaseState);
    }

    public override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
    }

    public override void Attack()
    {
        inAttack = true;
        anim.SetTrigger("attacking");
        direction = player.transform.position - transform.position;
    }
    
    public void AttackPoint()
    {
        GetComponent<Collider2D>().enabled = true;
        newpos = transform.position+(direction * armLength);
        collider2Ds = Physics2D.OverlapBoxAll(newpos, boxSize, playerMask);
        foreach(Collider2D collider in collider2Ds)
        {
            if(collider.CompareTag("Player"))
            {
                playerScript.OnDamamged(damage);
            }
        }
    }

    public void OutAttack()
    {
        inAttack = false;
    }


    private void OnDrawGizmos()
    {
        if(inAttack)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(newpos, boxSize);
        }
        
    }

    public void SpeedToZero()
    {
        agent.speed = 0;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = 0;
    }

    public void SpeedReturn()
    {
        agent.speed = moveSpeed;
    }
}