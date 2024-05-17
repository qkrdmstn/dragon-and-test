using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SkillProjectile : MonoBehaviour
{
    [Header("Projectile info")]
    public SkillStatusEffects statusEffect;
    public int damage;
    public float range;
    public Vector2 dir;
    public float projectileSpeed;

    #region Components
    private Rigidbody2D rigid;

    #endregion

    private void Awake()
    {
        statusEffect = new SkillStatusEffects();
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    public void Initialize(int _damage, float _range, Vector2 _dir, float _projectileSpeed, StatusEffect _statusEffect)
    {
        damage = _damage;
        range = _range;
        dir = _dir;
        projectileSpeed = _projectileSpeed;
        statusEffect.status = _statusEffect;
    }

    // Update is called once per frame
    void Update()
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

            if(statusEffect != null)
                statusEffect.ApplyStatusEffect(monster);

            monster.OnDamaged(damage);
        }
        if(collision.gameObject.CompareTag("Ground"))
        {
            InActiveProjectile();
        }
    }

    public void InActiveProjectile()
    {
        Destroy(this.gameObject);
    }
}
