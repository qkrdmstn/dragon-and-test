using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class SkillObj_Flame : SkillObject
{
    [SerializeField] private float damagePeriod;
    [SerializeField] private float timer;
    [SerializeField] private List<Collider2D> inRangeTarget;

    private Player player;
    private Collider2D collider2d;
    public SkillObj_Flame() : base()
    {
    }

    public void Initialize(int _damage, Vector2 _dir, StatusEffect _statusEffect, float _period)
    {
        player = FindObjectOfType<Player>();
        collider2d = GetComponent<Collider2D>();

        base.Initialize(_damage, _dir, _statusEffect);
        timer = 0.0f;
        damagePeriod = _period;
        SetDir();
    }

    private void FixedUpdate()
    {
        timer -= Time.deltaTime;
        if (timer < 0.0f)
        {
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask("Monster"));

            if (Physics2D.OverlapCollider(collider2d, filter.NoFilter(), inRangeTarget) != 0)
            {
                timer = damagePeriod;
                for (int i = 0; i < inRangeTarget.Count; i++)
                {
                    if (!inRangeTarget[i].CompareTag("Monster"))
                        continue;

                    MonsterBase monster = inRangeTarget[i].GetComponent<MonsterBase>();
                    Boss boss = inRangeTarget[i].GetComponent<Boss>();
                    Debug.Log("sss");
                    if (monster != null)
                        MonsterDamaged(monster);
                    else if (boss != null)
                        BossDamaged(boss);
                }
            }
        }
    }

    void Update()
    {
        SetDir();
        transform.position = player.transform.position;
    }

    public void MonsterDamaged(MonsterBase monster)
    {
        monster.OnDamaged(damage);

        if (statusEffect != null && statusEffect.status != StatusEffect.None)
            statusEffect.ApplyStatusEffect(monster);
    }

    public void BossDamaged(Boss boss)
    {
        boss.OnDamaged(damage);
    }

    private void SetDir()
    {
        //Initial Direction Setting
        dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
        dir.Normalize();
        float theta = Vector2.Angle(Vector2.right, dir);
        if (dir.y < 0)
            theta *= -1;

        this.transform.rotation = Quaternion.Euler(0, 0, theta);
    }
}
