using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaserBullet : MonoBehaviour
{
    [Header("Bullet Information")]
    public int damage;
    public float range;
    public float curDist;
    public float knockbackForce;
    public float bulletSpeed;
    public Vector2 dir;

    public Vector3 startPos;
    public Vector3 endPos;

    public bool flag;
    public LineRenderer lineRenderer;
    public int impactLayerMask;
    #region Components
    private Rigidbody2D rigid;

    #endregion

    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        impactLayerMask = LayerMask.GetMask("Monster");
    }

    private void FixedUpdate()
    {
        if(!flag)
        {
            RaycastHit2D hit = Physics2D.Raycast(Player.instance.transform.position, dir, range, impactLayerMask);
            if (hit.collider != null)
            {
                endPos = Vector3.Dot(dir, hit.collider.gameObject.transform.position - startPos) * dir;
                endPos += startPos;
                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, endPos);
                Damage(hit.collider);
            }
            else
            {
                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, endPos);

                InActiveBullet();
            }

        }

    }

    public void BulletInitialize(int _damage, float _range, float _speed, float _knockbackForce, Vector2 _dir)
    {
        damage = _damage;
        range = _range;
        bulletSpeed = _speed;
        knockbackForce = _knockbackForce;
        dir = _dir;

        curDist = 0;
        flag = false;

        startPos = Player.instance.transform.position;
        endPos = startPos + new Vector3(dir.x, dir.y, this.transform.position.z) * range;
    }

    private void Damage(Collider2D collision)
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
        StartCoroutine(InActiveBulletCoroutine());
    }

    IEnumerator InActiveBulletCoroutine()
    {
        flag = true;
        float dist = 0.0f;
        Vector3 pos = startPos;
        while (Vector3.Dot(endPos - pos, endPos - startPos) > 0.0f)
        {
            yield return null;

            dist = bulletSpeed * Time.deltaTime;
            pos += new Vector3(dir.x, dir.y, this.transform.position.z) * dist;
            lineRenderer.SetPosition(0, pos);
            lineRenderer.SetPosition(1, endPos);
        }

        Destroy(this.gameObject);
    }
}
