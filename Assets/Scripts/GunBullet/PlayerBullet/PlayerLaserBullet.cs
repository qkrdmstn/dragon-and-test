using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaserBullet : MonoBehaviour
{
    [Header("Bullet Information")]
    public int damage;
    public float range;
    public float knockbackForce;
    public float bulletSpeed;
    public float lifeTime;
    public Vector2 dir;

    private float timer;
    #region Components
    private Rigidbody2D rigid;

    #endregion

    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime < 0.0f)
            InActiveBullet();
    }

    public void BulletInitialize(int _damage, float _range, float _speed, float _knockbackForce, float _lifeTime, Vector2 _dir)
    {
        damage = _damage;
        range = _range;
        bulletSpeed = _speed;
        knockbackForce = _knockbackForce;
        lifeTime = _lifeTime;
        dir = _dir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            MonsterBase monster = collision.GetComponent<MonsterBase>();
            Boss boss = collision.GetComponent<Boss>();

            if (monster != null)
                monster.OnDamaged(damage);
            else if (boss != null)
                boss.OnDamaged(damage);
            InActiveBullet();
        }

        if (collision.gameObject.CompareTag("PuzzleTotem"))
        {
            StoneTotem stoneTotem = collision.GetComponent<StoneTotem>();
            stoneTotem.OnDamaged();
            InActiveBullet();
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            InActiveBullet();
        }
    }

    public void InActiveBullet()
    {
        Destroy(this.gameObject);
    }
}
