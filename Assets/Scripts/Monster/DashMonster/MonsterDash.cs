using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDash : MonsterBase
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
    #endregion

    #region States
    public MonsterIdleStateDash idleState { get; private set; }
    public MonsterChaseStateDash chaseState { get; private set; }
    public MonsterAttackStateDash attackState { get; private set; }
    public float chaseRange = 10.0f;
    public float attackRange = 4.0f;
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

        spriteRenderer = attackWarning.GetComponent<SpriteRenderer>();

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
            direction *= 5f;
        }
    }

    public override void Knockback(Vector2 dir, float vel)
    {
        if (!isKnockedBack && !inAttack)
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

        Invoke("AttackPoint", 0.9f);
        Invoke("OutAttack", 1.5f);
        //anim.SetTrigger("attacking");
    }
    
    public void AttackPoint()
    {
        warning = false;
        attackWarning.SetActive(false);
        SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.dashAttack);
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