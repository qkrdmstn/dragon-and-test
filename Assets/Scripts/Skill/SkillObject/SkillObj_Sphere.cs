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

    public void Initialize(int _damage, Vector2 _dir, float _projectileSpeed, float _range, float _period, float _slowScale)
    {
        rigid = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();

        base.Initialize(_damage, _dir);

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

                    MonsterBase monster = inRangeTarget[i].GetComponent<MonsterBase>();
                    monster.OnDamaged(damage);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Monster"))
            return;

        MonsterBase monster = collision.GetComponent<MonsterBase>();
        if (!monster.statusEffectsFlag.rooted && !monster.statusEffectsFlag.stun)
            monster.SetSlowSpeed(slowScale);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Monster"))
            return;

        MonsterBase monster = collision.GetComponent<MonsterBase>();
        if (!monster.statusEffectsFlag.rooted && !monster.statusEffectsFlag.stun)
            monster.SetSlowSpeed(slowScale);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Monster"))
            return;

        MonsterBase monster = collision.GetComponent<MonsterBase>();
        if (!monster.statusEffectsFlag.rooted && !monster.statusEffectsFlag.stun)
            monster.SetNormalSpeed();
    }
    private void Update()
    {
        timer -= Time.deltaTime;
    }

    public void InActiveProjectile()
    {
        Destroy(this.gameObject);
    }
}
