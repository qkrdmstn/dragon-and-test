using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterDash : MonsterBase
{
    [Header("MonsterDash---------------")]
    public float chargingSec = 0.7f;
    public float dashRatio = 2f;

    #region MonsterAttack
    [Header("MonsterAttack")]
    public bool inAttack = false;
    public bool warning = false;

    public float cooldown = 8.0f;
    public float attackSpeed = 10.0f;
    public float attackForce = 5f;
    public float tempcool;

    public Vector3 direction;

    public float chaseRange = 10.0f;
    public float attackRange = 4.0f;
    #endregion

    #region States
    public MonsterIdleStateDash idleState { get; private set; }
    public MonsterChaseStateDash chaseState { get; private set; }
    public MonsterAttackStateDash attackState { get; private set; }

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

        StartCoroutine(AnimSpawn());
        stateMachine.Initialize(chaseState);
    }

    public override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (inAttack)
        {
            if (warning)
                direction = (player.transform.position - transform.position);               
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
        StartCoroutine(ControlAttack());
    }

    IEnumerator ControlAttack()
    {
        // 공격 차징중
        monsterAnimController.SetAnim(MonsterAnimState.Attack, CheckDir());
        yield return new WaitForSeconds(chargingSec);

        // 공격
        warning = false;
        monsterAnimController.SetAnim(MonsterAnimState.Run, CheckDir());
        monsterAnimController.SetAnimSpeed(dashRatio);
        
        SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.dashAttack);
        
        rigidBody.AddForce(direction.normalized * attackForce * attackSpeed);
        yield return new WaitForSeconds(1f);
        // 공격 종료
        rigidBody.velocity = Vector3.zero;
        inAttack = false;

        monsterAnimController.SetAnimSpeed(1f);
    }
}