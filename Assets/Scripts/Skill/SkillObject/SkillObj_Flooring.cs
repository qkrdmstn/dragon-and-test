using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillObj_Flooring : SkillObject
{
    [SerializeField] private Vector2 targetPos;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float damagePeriod;
    [SerializeField] private float timer;
    [SerializeField] private bool isFlooring;
    [SerializeField] private Sprite floorImg;
    [SerializeField] private List<Collider2D> inRangeTarget;

    #region Components
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid;
    private Collider2D collider2d;
    #endregion

    public SkillObj_Flooring() : base()
    {
    }

    public void Initialize(int _damage, Vector2 _dir, Vector2 _targetPos, float _projectileSpeed, StatusEffect _statusEffect, float _period)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();

        base.Initialize(_damage, _dir, _statusEffect);
        targetPos = _targetPos;
        projectileSpeed = _projectileSpeed;
        damagePeriod = _period;
        isFlooring = false;
    }

    private void FixedUpdate()
    {
        if (!isFlooring) //투사체 형태
        {
            //투사체 이동
            rigid.velocity = dir * projectileSpeed;
            //위치 확인
            Vector2 pos = this.transform.position;
            if (Vector2.Distance(pos, targetPos) < 0.2f)
                ChangeFlooring();
        }
        else if (isFlooring && timer < 0.0f)
        {
            ContactFilter2D filter = new ContactFilter2D().NoFilter();
            filter.SetLayerMask(LayerMask.GetMask("Monster"));
            if (Physics2D.OverlapCollider(collider2d, filter, inRangeTarget) != 0)
            {
                timer = damagePeriod;
                for (int i = 0; i < inRangeTarget.Count; i++)
                {
                    if (!inRangeTarget[i].CompareTag("Monster"))
                        continue;

                    MonsterBase2 monster = inRangeTarget[i].GetComponent<MonsterBase2>();
                    SkillAttack(monster);
                }
            }
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
    }

    public void ChangeFlooring() //장판으로 변환
    {
        isFlooring = true;
        spriteRenderer.sprite = floorImg;
        rigid.velocity = Vector2.zero;
    }

    public override void SkillAttack(MonsterBase2 monster)
    {
        monster.OnDamaged(damage);

        if (statusEffect != null && statusEffect.status != StatusEffect.None)
            statusEffect.ApplyStatusEffect(monster);
    }


}
