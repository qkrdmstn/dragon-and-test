using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Life info")]
    public int curHP = 10;
    public int maxHP = 10;
    public bool isDead = false;

    [Header("Move info")]
    public float moveSpeed = 3.65f;

    private BossHPUI bossHPUI;

    #region Componets
    public Rigidbody2D rb { get; private set; }
    public Player player { get; private set; }
    #endregion

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        bossHPUI = FindObjectOfType<BossHPUI>();
    }

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
    }

    public void SetVelocity(Vector2 vel)
    {
        rb.velocity = vel;
    }

    //데미지 처리
    public virtual void OnDamaged(int damage)
    {
        SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.Damage);

        curHP -= damage;

        if (curHP <= 0)
        {
            Dead();
        }

        bossHPUI.UpdateHPUI(curHP, maxHP);
    }

    public void DotDamage(float duration, float interval, int perDamage)
    {
        StartCoroutine(DotDamageCoroutine(duration, interval, perDamage));
    }

    IEnumerator DotDamageCoroutine(float duration, float interval, int perDamage)
    {
        float timer = interval;
        while (duration >= 0.0f)
        {
            yield return null;
            timer -= Time.deltaTime;
            duration -= Time.deltaTime;
            if (timer < 0.0f)
            {
                timer = interval;
                curHP -= perDamage;
                bossHPUI.UpdateHPUI(curHP, maxHP);
                SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.Damage);
            }
        }
    }

    //죽음
    public virtual void Dead()
    {
        if (!isDead)
        {
            isDead = true;
            StopAllCoroutines();

            MonsterBase[] monsterBases = FindObjectsByType<MonsterBase>(FindObjectsSortMode.None);
            BossBullet_Jan[] bossBullets = FindObjectsByType<BossBullet_Jan>(FindObjectsSortMode.None);
            Debug.Log("Dead!!!!!!!");

            for (int i = 0; i < monsterBases.Length; i++)
                monsterBases[i].Dead();

            for (int i = 0; i < bossBullets.Length; i++)
                Destroy(bossBullets[i].gameObject);
        }
    }
}
