
using UnityEngine;

public class TutorialFar : MonsterFar
{
    [Header("Tutorial----------")]
    public TutorialMonsters myNum;
    public TutorialInteraction tutorial;
    public bool isAttackable = false;

    public override void Awake()
    {
        base.Awake();
        tutorial = FindObjectOfType<TutorialInteraction>();
    }
    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Attack()
    {
        if (isAttackable)
            base.Attack();
    }

    public void SetAttackable() => isAttackable = true;

    public override void OnDamaged(int damage)
    {
        if (myNum == TutorialMonsters.attack)
        {
            if (!tutorial.isAttacked && tutorial.interactionF)
            {
                tutorial.isAttacked = true;
                return;
            }
            else if (!tutorial.isDashed)
                return;
        }
        else if (myNum == TutorialMonsters.skill && !tutorial.useSkill) return;

        base.OnDamaged(damage);
    }

    public override void Dead()
    {
        tutorial.monsters[(int)myNum].isKilled = true;
        SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.Dead);

        gameObject.SetActive(false);
    }

    public void ChaseState(bool state)
    {
        isChase = state;

        if (state) SpeedReturn();
        else SpeedToZero();
    }
}
