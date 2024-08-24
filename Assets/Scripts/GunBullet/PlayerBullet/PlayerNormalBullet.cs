using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNormalBullet : MonoBehaviour
{
    [Header("Bullet Information")]
    public int damage;
    public float range;
    public float curDist;
    public float knockbackForce;
    public float bulletSpeed;
    public Vector2 dir;

    #region Components
    private Rigidbody2D rigid;

    #endregion

    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rigid.velocity = dir * bulletSpeed;
        curDist += bulletSpeed * Time.deltaTime;

        if(curDist >= range)
            Destroy(this.gameObject);
    }

    public void BulletInitialize(int _damage, float _range, float _speed, float _knockbackForce, Vector2 _dir)
    {
        damage = _damage;
        range = _range;
        bulletSpeed = _speed;
        knockbackForce = _knockbackForce;
        dir = _dir;
        
        curDist = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            MonsterBase monster = collision.GetComponent<MonsterBase>();
            Boss boss = collision.GetComponent<Boss>();

            if (monster != null)
            {
                monster.OnDamaged(damage);

                Vector2 dir = this.transform.position - Player.instance.transform.position;
                dir.Normalize();
                monster.Knockback(dir, knockbackForce);
            }
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
