using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet_Jan : MonoBehaviour
{
    [Header("Bullet Information")]
    public float lifeTimer;
    public float lifeTime;
    public int damage;

    [Header("Bullet Movement")]
    public Vector2 dir;
    public float bulletSpeed = 1f;

    private Rigidbody2D rigid;

    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        lifeTimer -= Time.deltaTime;
        rigid.velocity = dir.normalized * bulletSpeed;

        if (!IsInDomain() || lifeTimer < 0.0f)
        {
            Destroy(this.gameObject);
        }
    }

    public void BulletInitialize(Vector2 _dir, float _speed)
    {
        dir = _dir;
        lifeTimer = lifeTime;
        bulletSpeed = _speed;

        float theta = Vector2.Angle(Vector2.right, _dir);
        if (_dir.y < 0)
            theta *= -1;
        this.transform.rotation = Quaternion.Euler(0, 0, theta);
    }

    public void BulletInitialize(Vector2 _dir, float _speed, float _lifeTime)
    {
        dir = _dir;
        lifeTimer = _lifeTime;
        bulletSpeed = _speed;

        float theta = Vector2.Angle(Vector2.right, _dir);
        if (_dir.y < 0)
            theta *= -1;
        this.transform.rotation = Quaternion.Euler(0, 0, theta);
    }

    public void BulletInitialize(Vector2 _dir)
    {
        dir = _dir;
        lifeTimer = lifeTime;

        float theta = Vector2.Angle(Vector2.right, _dir);
        if (_dir.y < 0)
            theta *= -1;
        this.transform.rotation = Quaternion.Euler(0, 0, theta);
    }

    private bool IsInDomain()
    {
        return transform.position.x <= 100 && transform.position.x >= -100 && transform.position.y <= 100 && transform.position.y >= -100;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ground"))
        {
            if (other.CompareTag("Player") && Player.instance.IsDash())
            {
                return;
            }

            Destroy(this.gameObject);
        }
    }
}
