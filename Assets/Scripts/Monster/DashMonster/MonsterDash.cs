using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDash : MonsterBase
{
    
    #region MonsterAttack
    public bool inAttack = false;
    public float cooldown = 8.0f;
    public float attackSpeed = 10.0f;
    public float tempcool;
    public Vector3 direction;
    #endregion

    #region States
    public MonsterIdleStateDash idleState { get; private set; }
    public MonsterChaseStateDash chaseState { get; private set; }
    public MonsterAttackStateDash attackState { get; private set; }
    public float chaseRange = 10.0f;
    public float attackRange = 4.0f;
    
    public float distanceToPlayer;
    #endregion

    #region Navigate
    private UnityEngine.AI.NavMeshAgent agent;
    #endregion

    public override void Awake()
    {
        base.Awake();
        idleState = new MonsterIdleStateDash(stateMachine, player, this);
        chaseState = new MonsterChaseStateDash(stateMachine, player, this);
        attackState = new MonsterAttackStateDash(stateMachine, player, this);
    }

    public override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        SpeedToZero();
    }

    public override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        tempcool -= Time.deltaTime;

        //navigate
        agent.SetDestination(player.transform.position);
    }

    public override void Attack()
    {
        inAttack = true;
        anim.SetTrigger("attacking");
        direction = player.transform.position - transform.position;
    }
    
    public void AttackPoint()
    {
        rigidBody.AddForce(direction * attackSpeed);
    }

    public void OutAttack()
    {
        rigidBody.velocity = Vector3.zero;
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