using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BirdWarrior2 : MonsterBase
{
    [Header("---------------BirdWarrior2---------------")]
    [Header("Attack Info")]
    public float chargingDelay = 0.7f;
    public float dashDist = 5.0f;
    public float dashSpeed = 8.0f;
    public float dashDelay = 0.35f;
    public float knockbackForce = 10.0f;
    public bool isDashing = false;
    public GameObject warningObject;
    
    [Header("Anim Info")]
    public float dashRatio = 2.0f;

    #region Addtional States
    public MonsterAttackState_BirdWarrior2 attackState;
    #endregion

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(spawnState);
    }

    public override void InitStates()
    {
        base.InitStates();

        spawnState = new MonsterSpawnStateBase(stateMachine, player, this);
        idleState = new MonsterIdleStateBase(stateMachine, player, this);
        deadState = new MonsterDeadStateBase(stateMachine, player, this);

        chaseState = new MonsterChaseState_BirdWarrior2(stateMachine, player, this);
        attackState = new MonsterAttackState_BirdWarrior2(stateMachine, player, this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isDashing)
        {
            player.PlayerKnockBack(player.transform.position - transform.position, knockbackForce); //Vector2 dir, float mag
        }
    }
}