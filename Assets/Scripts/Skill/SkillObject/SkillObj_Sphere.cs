using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObj_Sphere : SkillObject
{
    [SerializeField] private float range;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float damagePeriod;
    [SerializeField] private float timer;
    [SerializeField] private List<Collider2D> inRangeTarget;
    [SerializeField] private float slowScale;

    private Rigidbody2D rigid;
    private Collider2D collider2d;

    public void Initialize(int _damage, Vector2 _dir, float _projectileSpeed, float _range, StatusEffect _statusEffect, float _period, float _slowScale)
    {
        rigid = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();

        base.Initialize(_damage, _dir, _statusEffect);

        range = _range;
        projectileSpeed = _projectileSpeed;
        timer = 0.0f;
        damagePeriod = _period;
        slowScale = _slowScale;
    }

    private void FixedUpdate()
    {
        rigid.velocity = dir * projectileSpeed;
        range -= rigid.velocity.magnitude * Time.deltaTime;
        if (range < 0.0f)
            InActiveProjectile();

        if (timer < 0.0f)
        {
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask("Monster"));

            if (Physics2D.OverlapCollider(collider2d, filter.NoFilter(), inRangeTarget) != 0)
            {
                timer = damagePeriod;
                for (int i = 0; i < inRangeTarget.Count; i++)
                {
                    if (!inRangeTarget[i].CompareTag("Monster"))
                        continue;

                    MonsterBase2 monster = inRangeTarget[i].GetComponent<MonsterBase2>();
                    Boss boss = inRangeTarget[i].GetComponent<Boss>();
                    if (monster != null)
                        MonsterDamaged(monster);
                    else if (boss != null)
                        BossDamaged(boss);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Monster"))
            return;

        MonsterBase2 monster = collision.GetComponent<MonsterBase2>();
        Boss boss = collision.GetComponent<Boss>();
        if (monster != null)
            monster.SetSlowSpeed(slowScale);
        else if (boss != null)
        {
            boss.moveSpeed = 3.65f - 3.65f * slowScale;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Monster"))
            return;

        MonsterBase2 monster = collision.GetComponent<MonsterBase2>();
        Boss boss = collision.GetComponent<Boss>();
        if (monster != null)
            monster.SetNormalSpeed();
        else if (boss != null)
        {
            boss.moveSpeed = 3.65f;
        }
            
    }
    private void Update()
    {
        timer -= Time.deltaTime;
    }

    public void MonsterDamaged(MonsterBase2 monster)
    {
        monster.OnDamaged(damage);

        if (statusEffect != null && statusEffect.status != StatusEffect.None)
            statusEffect.ApplyStatusEffect(monster);
    }

    public void BossDamaged(Boss boss)
    {
        boss.OnDamaged(damage);
    }

    public void InActiveProjectile()
    {
        Destroy(this.gameObject);
    }
}
