using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonsterBase
{
    private BossHPUI bossHPUI;

    protected override void Awake()
    {
        base.Awake();
        bossHPUI = FindObjectOfType<BossHPUI>();
    }

    //데미지 처리
    public override void OnDamaged(int damage)
    {
        base.OnDamaged(damage);
        bossHPUI.UpdateHPUI(curHP, maxHP);
    }

    public override void InitComponents()
    {
        base.InitComponents();
    }

    public override void InitStates()
    {
        base.InitStates();
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
                monsterBases[i].stateMachine.ChangeState(monsterBases[i].deadState);

            for (int i = 0; i < bossBullets.Length; i++)
                Destroy(bossBullets[i].gameObject);
        }
    }
}
