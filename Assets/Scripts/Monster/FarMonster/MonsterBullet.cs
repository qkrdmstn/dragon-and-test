using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Pool;

public class MonsterBullet : MonoBehaviour
{

    [Header("Bullet Information")]
    public float lifeTimer;
    public float lifeTime;
    public int damage;
    //public int bounce;

    [Header("Bullet Movement")]
    public Vector2 dir;
    //public float bulletSpeed = 1f;
    public float normalSpeed = 5f;
    private float speed;
    public bool isRelease;
    public bool isPooled;
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
        lifeTimer -= Time.deltaTime;
        rigid.velocity = dir.normalized * speed;

        if ((!IsInDomain() || lifeTimer < 0.0f) && !isRelease)
        {
            if (isPooled)
            {
                isRelease = true;
                pool.Release(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        
    }

    public void BulletInitialize(Vector2 _dir, float _speed)
    {
        isRelease = false;
        dir = _dir;
        lifeTimer = lifeTime;
        speed = _speed;
        
        float theta = Vector2.Angle(Vector2.right, _dir);
        if (_dir.y < 0)
            theta *= -1;
        this.transform.rotation = Quaternion.Euler(0, 0, theta);
    }

    public void BulletInitialize(Vector2 _dir)
    {
        isRelease = false;
        dir = _dir;
        lifeTimer = lifeTime;
        speed = normalSpeed;

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

            if (!isRelease)
            {
                if(isPooled)
                {
                    isRelease = true;
                    pool.Release(gameObject);
                }
                else
                {
                    Destroy(this.gameObject);
                }
                
            }
        }
    }
}
