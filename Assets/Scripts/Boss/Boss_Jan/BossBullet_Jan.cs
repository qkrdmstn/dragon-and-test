using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet_Jan : MonoBehaviour
{
    [Header("Bullet Information")]
    public int damage;

    [Header("Bullet Movement")]
    public Vector2 dir;
    public float range;
    public float curDist;
    public float bulletSpeed = 1f;
    private Rigidbody2D rigid;

    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rigid.velocity = dir.normalized * bulletSpeed;
        curDist += bulletSpeed * Time.deltaTime;

        if (curDist >= range)
            Destroy(this.gameObject);
    }

    void Update()
    {
        if (!IsInDomain() )
        {
            Destroy(this.gameObject);
        }
    }

    public void BulletInitialize(Vector2 _dir)
    {
        dir = _dir;

        float theta = Vector2.Angle(Vector2.right, _dir);
        if (_dir.y < 0)
            theta *= -1;
        this.transform.rotation = Quaternion.Euler(0, 0, theta);
    }

    public void BulletInitialize(Vector2 _dir, float _speed)
    {
        dir = _dir;
        bulletSpeed = _speed;

        float theta = Vector2.Angle(Vector2.right, _dir);
        if (_dir.y < 0)
            theta *= -1;
        this.transform.rotation = Quaternion.Euler(0, 0, theta);
    }

    public void BulletInitialize(Vector2 _dir, float _speed, float _range)
    {
        dir = _dir;
        bulletSpeed = _speed;
        range = _range;
        curDist = 0;

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
