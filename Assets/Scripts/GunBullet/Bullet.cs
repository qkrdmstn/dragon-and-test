using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTimer;

    [Header("Bullet Information")]
    public int damage;
    //public int bounce;
    public float lifeTime;

    [Header("Bullet Movement")]
    public Vector2 dir;
    public float bulletSpeed;

    #region Components
    private Rigidbody2D rigid;

    #endregion

    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        lifeTimer -= Time.deltaTime;

        rigid.velocity = dir * bulletSpeed;

        //�ӽ� Bound
        if (!IsInDomain() || lifeTimer < 0.0f)
            Destroy(this.gameObject);
    }

    public void BulletInitialize(int _damage, Vector2 _dir)
    {
        damage = _damage;
        dir = _dir;
        lifeTimer = lifeTime;
    }

    private bool IsInDomain()
    {
        return transform.position.x <= 100 && transform.position.x >= -100 && transform.position.y <= 100 && transform.position.y >= -100;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster") || collision.gameObject.CompareTag("Ground"))
        {
            Destroy(this.gameObject);
        }
    }
}
