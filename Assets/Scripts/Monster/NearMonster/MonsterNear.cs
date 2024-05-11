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

    public Chase chase;
    public float tempSpeed;
    public float distanceToPlayer;

    public override void Awake()
    {
        base.Awake();
        chaseState = new MonsterChaseStateNear(stateMachine, player, this);
        attackState = new MonsterAttackStateNear(stateMachine, player, this);
        tempSpeed = moveSpeed;
    }

    public override void Start()
    {
        base.Start();
        stateMachine.Initialize(chaseState);

        chase = GetComponent<Chase>();
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
    }
    
    public void AttackPoint()
    {
        GetComponent<Collider2D>().enabled = true;
        newpos = transform.position+(Vector3)(chase.tempDir * armLength);
        collider2Ds = Physics2D.OverlapBoxAll(newpos, boxSize, Mathf.Atan2(chase.tempDir.y, chase.tempDir.x) * Mathf.Rad2Deg, playerMask);
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
        if(newpos != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(newpos, boxSize);
        }
        
    }
}