using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRage : MonsterBase
{
    
    #region MonsterAttack
    public bool inAttack = false;
    public float cooldown = 2.0f;
    public float tempcool;
    public Collider2D[] collider2Ds;
    public Vector2 boxSize;
    public float armLength;
    private Vector3 newpos;
    public LayerMask playerMask;  
    #endregion

    #region States
    public MonsterChaseStateRage chaseState { get; private set; }
    public MonsterRageState rageState { get; private set; }
    public float attackRange = 1.0f;
    public float rageRange = 5.0f;
    public bool rageAble = false;
    #endregion

    public Vector3 direction;

    public override void Awake()
    {
        base.Awake();
        chaseState = new MonsterChaseStateRage(stateMachine, player, this);
        rageState = new MonsterRageState(stateMachine, player, this);
    }

    public override void Start()
    {
        base.Start();

        SpeedToZero();
        stateMachine.Initialize(chaseState);
        Invoke("RageAble", 0.5f);
    }

    public override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        agent.SetDestination(player.transform.position);
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.gameObject.CompareTag("Bullet"))
        {
            stateMachine.ChangeState(rageState);
        }
    }

    public override void Attack()
    {
        inAttack = true;
        //anim.SetTrigger("attacking");
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

    public void SpeedBoost()
    {
        agent.speed = moveSpeed * 3;
    }

    private void RageAble()
    {
        rageAble = true;
    }
}