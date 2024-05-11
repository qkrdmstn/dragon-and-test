using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MonsterBullet : MonoBehaviour
{

    [Header("Bullet Information")]
    public int damage;
    //public int bounce;

    [Header("Bullet Movement")]
    public Vector2 dir;
    public float bulletSpeed;

    #region Components
    private Rigidbody2D rigid;
    public IObjectPool<GameObject> pool { get; set; }

    #endregion

    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rigid.velocity = dir * bulletSpeed;

        if (!IsInDomain())
            pool.Release(this.gameObject);
    }

    public void BulletInitialize(Vector2 _dir)
    {
        dir = _dir;
    }

    private bool IsInDomain()
    {
        return transform.position.x <= 100 && transform.position.x >= -100 && transform.position.y <= 100 && transform.position.y >= -100;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ground"))
        {
            pool.Release(gameObject);
        }
    }
}
