using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterNear : MonsterBase2
{
    [Header("MonsterNear---------------")]
    public GameObject swordAura;

    #region MonsterAttack
    [Header("MonsterAttack")]
    public bool inAttack = false;
    public float cooldown = 3.0f;
    public float tempcool;
    public float attackDuration = 0.3f;
    public float attackRange = 1.0f;
    float[] directions = { 0, 45, 90, 180, 270, 315, 360};
    int shootNumber = 0;
    #endregion

    public bool isTanker = false;

    #region States
    public MonsterChaseStateNear chaseState { get; private set; }
    public MonsterAttackStateNear attackState { get; private set; }
    #endregion

    public override void Awake()
    {
        base.Awake();
        chaseState = new MonsterChaseStateNear(stateMachine, player, this);
        attackState = new MonsterAttackStateNear(stateMachine, player, this);
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
    }

    public override void Attack()
    {
        inAttack = true;

        AttackPoint();
    }
    
    public virtual void AttackPoint()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction.Normalize();

        float angle = Quaternion.FromToRotation(Vector3.up, direction).eulerAngles.z;
        float closestDirection = directions[0];
        float minDifference = Mathf.Abs(angle - directions[0]);

        for (int i = 1; i < directions.Length; i++)
        {
            float difference = Mathf.Abs(angle - directions[i]);
            if (difference < minDifference)
            {
                minDifference = difference;
                closestDirection = directions[i];
            }
        }

        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        shootNumber = 0;
        while (shootNumber < 3)
        {
            Vector3 dir = player.transform.position - transform.position;

            GameObject aura = Instantiate(swordAura, transform.position, Quaternion.identity);
            MonsterBullet auraScript = aura.GetComponent<MonsterBullet>();
            SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.nearAttack);
            auraScript.BulletInitialize(dir);
            ++shootNumber;
            yield return new WaitForSeconds(0.7f);
        }
        OutAttack();
    }

    public void OutAttack()
    {
        inAttack = false;
    }
    public override void Dead()
    {
        if (!isDead)
        {
            isChase = false;    
            isDead = true;
            SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.Dead);

            StartCoroutine(AnimDead());
        }
    }
}