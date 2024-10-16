using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterNear : MonsterBase
{
    [Header("MonsterNear---------------")]
    public GameObject sword;
    public GameObject swordAura;
    BoxCollider2D swordCollider;

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
        swordCollider = sword.GetComponent<BoxCollider2D>();

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


        //switch (closestDirection)
        //{
        //    case 0:
        //    case 45:
        //    case 180:
        //        StartCoroutine(RotateOverTime(25, 325, attackDuration));
        //        break;

        //    case 90:
        //    case 270:
        //    case 315:
        //        StartCoroutine(RotateOverTime(325, 25, attackDuration));
        //        break;
        //}

        //if (closestDirection <= 180)
        //{
        //    //sword.transform.rotation = Quaternion.Euler(0, 0, (closestDirection-135));
        //    StartCoroutine(RotateOverTime(90, attackDuration));
        //}
        //else
        //{
        //    //sword.transform.rotation = Quaternion.Euler(0, 0, (closestDirection-45));
        //    StartCoroutine(RotateOverTime(-90, attackDuration));
        //}
        
        StartCoroutine(Shoot());
        //InvokeRepeating("Shoot", 0f, 0.6f);
    }

    IEnumerator RotateOverTime(float startAngle, float endAngle, float duration)
    {
        bool isDamagedOnce = false;
        float startTime = Time.time;
        Quaternion startRotation = Quaternion.Euler(0, 0, startAngle);
        Quaternion endRotation = Quaternion.Euler(0, 0, endAngle);

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            sword.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);

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

        //sword.transform.rotation = endRotation;
    }

    //public void Shoot()
    //{
    //    if (shootNumber == 3)
    //    {
    //        CancelInvoke("Shoot");
    //        OutAttack();
    //        return;
    //    }

    //    Vector3 dir = player.transform.position-transform.position;

    //    GameObject aura = Instantiate(swordAura, transform.position, Quaternion.identity);
    //    MonsterBullet auraScript = aura.GetComponent<MonsterBullet>();
        
    //    auraScript.BulletInitialize(dir);
    //    ++shootNumber;
    //}

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