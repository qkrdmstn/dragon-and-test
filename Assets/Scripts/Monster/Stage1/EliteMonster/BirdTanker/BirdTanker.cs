using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BirdTanker : MonsterBase
{
    [Header("---------------BirdTanker---------------")]
    [Header("Attack Info")]
    public int waveNum = 3;
    public int bulletNumPerWave = 28;
    public float waveInterval = 1.0f;
    public float bulletSpeed = 6.0f;
    public float reloadDelay = 1.0f;
    public float dashSpeed = 8.0f;
    public float dashDist = 1.5f;

    #region Addtional States
    public MonsterAttackState_BirdTanker attackState;
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

        chaseState = new MonsterChaseState_BirdTanker(stateMachine, player, this);
        attackState = new MonsterAttackState_BirdTanker(stateMachine, player, this);
    }
}