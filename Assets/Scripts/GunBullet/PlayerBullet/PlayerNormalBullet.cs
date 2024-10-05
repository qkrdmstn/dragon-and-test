using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    public void BulletInitialize(int _damage, float _range, float _speed, float _knockbackForce, Vector2 _dir, float _scale)
    {
        damage = _damage;
        range = _range;
        bulletSpeed = _speed;
        knockbackForce = _knockbackForce;
        dir = _dir;
        transform.localScale = transform.lossyScale * _scale;

        curDist = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            MonsterBase2 monster = collision.GetComponent<MonsterBase2>();
            Boss boss = collision.GetComponent<Boss>();

            if (monster != null)
                MonsterDamaged(monster);
            else if (boss != null)
                BossDamaged(boss);
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

    public void MonsterDamaged(MonsterBase2 monster)
    {
        monster.OnDamaged(damage);

        //알리
        SkillDB al12Data = SkillManager.instance.GetSkillDB(SeotdaHwatuCombination.AL12);
        float al12Prob = SkillManager.instance.GetSkillProb(SeotdaHwatuCombination.AL12);
        float randomVal = Random.Range(0.0f, 1.0f);
        if (SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.AL12) && randomVal <= al12Prob)
        {
            PlayerSkill playerSkill = Player.instance.GetComponent<PlayerSkill>();
            playerSkill.AL12Effect(monster.gameObject, damage);
        }

        //독사
        SkillDB ds14Data = SkillManager.instance.GetSkillDB(SeotdaHwatuCombination.DS14);
        float ds14Prob = SkillManager.instance.GetSkillProb(SeotdaHwatuCombination.DS14);
        randomVal = Random.Range(0.0f, 1.0f);
        if (SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.DS14) && randomVal <= ds14Prob)
        {
            //지속 데미지 -> duration과 interval이 없음
            monster.DotDamage(ds14Data.duration, ds14Data.period, ds14Data.damage);
        }

        //구삥
        SkillDB gpp19Data = SkillManager.instance.GetSkillDB(SeotdaHwatuCombination.GPP19);
        float gpp19Prob = SkillManager.instance.GetSkillProb(SeotdaHwatuCombination.GPP19);
        randomVal = Random.Range(0.0f, 1.0f);
        if (SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.GPP19) && randomVal <= gpp19Prob)
        {
            monster.EffectState();
        }

        Vector2 dir = this.transform.position - Player.instance.transform.position;
        dir.Normalize();
        monster.Knockback(dir, knockbackForce);
    }

    public void BossDamaged(Boss boss)
    {
        //암행어사
        SkillDB ahes74Data = SkillManager.instance.GetSkillDB(SeotdaHwatuCombination.AHES74);
        if (SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.AHES74))
            boss.OnDamaged(damage + (int)SkillManager.instance.GetSkillProb(SeotdaHwatuCombination.AHES74));
        else
            boss.OnDamaged(damage);

        //알리
        SkillDB al12Data = SkillManager.instance.GetSkillDB(SeotdaHwatuCombination.AL12);
        float al12Prob = SkillManager.instance.GetSkillProb(SeotdaHwatuCombination.AL12);
        float randomVal = Random.Range(0.0f, 1.0f);
        if (SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.AL12) && randomVal <= al12Prob)
        {
            PlayerSkill playerSkill = Player.instance.GetComponent<PlayerSkill>();
            playerSkill.AL12Effect(boss.gameObject, damage);
        }

        //독사
        SkillDB ds14Data = SkillManager.instance.GetSkillDB(SeotdaHwatuCombination.DS14);
        float ds14Prob = SkillManager.instance.GetSkillProb(SeotdaHwatuCombination.DS14);
        randomVal = Random.Range(0.0f, 1.0f);
        if (SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.DS14) && randomVal <= ds14Prob)
        {
            //지속 데미지 -> duration과 interval이 없음
            boss.DotDamage(ds14Data.duration, ds14Data.period, ds14Data.damage);
        }
    }
}
