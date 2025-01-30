using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillObj_Missile : SkillObject
{
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private float angle;
    private Transform target;
    private bool isTargeting;

    private Rigidbody2D rigid;
    private Transform tf;
    private Player player;
    private int monsterLayerMask;
    public SkillObj_Missile() : base()
    {

    }

    public void Initialize(int _damage, Vector2 _dir, float _speed, float _range, float _angle)
    {
        player = FindObjectOfType<Player>();
        rigid = GetComponent<Rigidbody2D>();
        tf = GetComponent<Transform>();
        base.Initialize(_damage, _dir);

        speed = _speed;
        range = _range;
        angle = _angle;

        monsterLayerMask = LayerMask.GetMask("Monster");
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        SetTarget();
        ChaseTarget();
    }

    private void SetTarget()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(tf.position, range, monsterLayerMask);
        float minDist = 10000;
        for (int i = 0; i < targets.Length; i++)
        {
            float dist = Vector2.Distance(targets[i].transform.position, tf.position);
            if (dist < minDist)
            {
                minDist = dist;
                target = targets[i].transform;
                isTargeting = true;
            }
        }
    }

    private void ChaseTarget()
    {
        if(!isTargeting)
        {
            angle += 4;
            Vector2 direction = Quaternion.AngleAxis(angle, Vector3.forward) * new Vector2(1, 0);
            Vector3 pos = player.transform.position + (Vector3)(direction * 1);
            tf.position = pos;

            float theta = Vector2.Angle(Vector2.right, direction);
            if (direction.y < 0)
                theta *= -1;
            tf.rotation = Quaternion.Euler(0, 0, theta);
        }
        else
        {
            dir = target.position - tf.position;
            dir.Normalize();
            float theta = Vector2.Angle(Vector2.right, dir);
            if (dir.y < 0)
                theta *= -1;

            tf.rotation = Quaternion.Euler(0, 0, theta);
            rigid.velocity = speed * dir;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            MonsterBase monster = collision.GetComponent<MonsterBase>();
            monster.OnDamaged(damage);
            InActiveProjectile();
        }

        if(isTargeting && collision.gameObject.CompareTag("Ground"))
                InActiveProjectile();
    }

    public void InActiveProjectile()
    {
        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, range);
    }
}
