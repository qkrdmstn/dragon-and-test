using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SkillObj_Vine : SkillObject
{
    [SerializeField] private float range;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float rootedDuration;

    #region Components
    public Rigidbody2D rigid;
    #endregion

    public SkillObj_Vine() : base()
    {
    }

    public void Initialize(int _damage, float _range, Vector2 _dir, float _projectileSpeed, float _rootedDuration)
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        base.Initialize(_damage, _dir);
        range = _range;
        projectileSpeed = _projectileSpeed;
        rootedDuration = _rootedDuration;
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
            monster.OnDamaged(damage);
            monster.Rooted(rootedDuration);
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            InActiveProjectile();
        }
    }

    public void InActiveProjectile()
    {
        Destroy(this.gameObject);
    }
}

