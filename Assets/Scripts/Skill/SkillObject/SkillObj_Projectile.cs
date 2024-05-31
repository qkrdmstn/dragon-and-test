using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SkillObj_Projectile : SkillObject
{
    [SerializeField] private float range;
    [SerializeField] private float projectileSpeed;

    #region Components
    public Rigidbody2D rigid;
    #endregion

    public SkillObj_Projectile() : base()
    {
    }

    public void Initialize(int _damage, float _range, Vector2 _dir, float _projectileSpeed, StatusEffect _statusEffect)
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        base.Initialize(_damage, _dir, _statusEffect);
        range = _range;
        projectileSpeed = _projectileSpeed;
    }

    private void Update()
    {
        rigid.velocity = dir * projectileSpeed;

        range -= rigid.velocity.magnitude * Time.deltaTime;
        if (range < 0.0f)
            InActiveProjectile();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            MonsterBase monster = collision.GetComponent<MonsterBase>();

            SkillAttack(monster);

            if (statusEffect != null || statusEffect.status != StatusEffect.None)
                statusEffect.ApplyStatusEffect(monster);
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            InActiveProjectile();
        }
    }

    public override void SkillAttack(MonsterBase monster)
    {
        monster.OnDamaged(damage);
    }

    public void InActiveProjectile()
    {
        Destroy(this.gameObject);
    }

}

