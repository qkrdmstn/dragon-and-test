using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType
{
    Archer, // 매
    Warrior // 참새 
}

public class MonsterTutorial : MonoBehaviour
{
    public Rigidbody2D rigidBody { get; private set; }

    #region Stats
    public MonsterType myType;
    public int HP;
    public int damage;
    public float moveSpeed;         
    #endregion

    #region KnockBack
    public float knockbackForce;    // 넉백 힘
    public float knockbackDuration; // 넉백 지속 시간
    public bool isKnockedBack;      // 넉백 상태 여부를 나타내는 변수
    public float knockbackTimer;    // 넉백 지속 시간을 계산하는 타이머
    public Vector2 knockbackVel;
    #endregion

    GameObject player;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        player = GameManager.instance.player.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (isKnockedBack)
        {
            knockbackTimer += Time.deltaTime;
            //Exponantial
            knockbackVel = knockbackVel * Mathf.Exp(-0.1f * knockbackTimer);
            rigidBody.velocity = knockbackVel;
            if (rigidBody.velocity.magnitude <= 0.1f)
            {
                rigidBody.velocity = Vector2.zero;
                isKnockedBack = false;
            }
        }
    }

    //피격
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            OnDamaged(1);

            Vector2 dir = this.transform.position - player.transform.position;
            dir.Normalize();
            Knockback(dir, knockbackForce);
        }
    }

    //데미지 처리
    public void OnDamaged(int damage)
    {
        //피격에 따른 카메라 진동
        //  CameraManager.instance.CameraShakeFromProfile(profile, impulseSource);

        HP -= damage;
        if (HP <= 0)
        {
            Dead();
        }
    }

    public void Knockback(Vector2 dir, float vel)
    {
        if (!isKnockedBack)
        {
            isKnockedBack = true;
            knockbackTimer = 0.0f;
            dir.Normalize();
            knockbackVel = vel * dir;
            rigidBody.velocity = knockbackVel;
            //rigidBody.velocity = Vector2.zero; // 현재 속도를 초기화
            //rigidBody.AddForce(dir * force, ForceMode2D.Impulse); // 총알 방향으로 힘을 가함
        }
    }

    //죽음
    public void Dead()
    {
        Destroy(gameObject);
    }
}
