using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNear : MonsterBase
{
    
    #region MonsterAttack
    public bool inAttack = false;
    public float cooldown = 3.0f;
    public float tempcool;
    public LayerMask playerMask;  
    private Collider2D swordCollider;
    public float attackDuration = 0.3f;
    private float[] directions = { 0, 45, 90, 180, 270, 315, 360};
    private int shootNumber = 0;
    public GameObject swordAura;
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
    public GameObject sword;

    public override void Awake()
    {
        base.Awake();
        chaseState = new MonsterChaseStateNear(stateMachine, player, this);
        attackState = new MonsterAttackStateNear(stateMachine, player, this);
    }

    public override void Start()
    {
        base.Start();

        swordCollider = sword.GetComponent<Collider2D>();
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
    }
    
    public virtual void AttackPoint()
    {
        swordCollider.enabled = true;

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
        

        if (closestDirection <= 180)
        {
            sword.transform.rotation = Quaternion.Euler(0, 0, (closestDirection-135));
            StartCoroutine(RotateOverTime(90, attackDuration));
        }
        else
        {
            sword.transform.rotation = Quaternion.Euler(0, 0, (closestDirection-45));
            StartCoroutine(RotateOverTime(-90, attackDuration));
        }
        
        shootNumber = 0;
        InvokeRepeating("Shoot", 0f, 0.3f);
    }

    IEnumerator RotateOverTime(float angle, float duration)
    {
        float startTime = Time.time;
        Quaternion startRotation = sword.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(sword.transform.eulerAngles + new Vector3(0, 0, angle));
        bool isDamagedOnce = false;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            sword.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(swordCollider.bounds.center, swordCollider.bounds.size, 0);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Player") && !isDamagedOnce)
                {
                    isDamagedOnce = true;
                    playerScript.OnDamamged(1);
                }
            }
            yield return null;
        }

        sword.transform.rotation = endRotation;
    }

    public void Shoot()
    {
        Vector3 dir = player.transform.position-transform.position;

        GameObject aura = Instantiate(swordAura, transform.position, Quaternion.identity);
        MonsterBullet auraScript = aura.GetComponent<MonsterBullet>();
        
        auraScript.BulletInitialize(dir);
        shootNumber += 1;
        if (shootNumber==3) CancelInvoke("Shoot");
                
    }

    public void OutAttack()
    {
        inAttack = false;
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