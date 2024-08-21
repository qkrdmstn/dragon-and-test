
using UnityEngine;

public class TutorialFar : MonsterFar
{
    [Header("Tutorial----------")]
    public TutorialInteraction.TutorialMonsters myNum;
    public TutorialInteraction tutorial;

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

    public override void OnDamaged(int damage)
    {
        if (myNum == TutorialInteraction.TutorialMonsters.attack)
        {
            if (!tutorial.isAttacked)
            {
                tutorial.isAttacked = true;
                return;
            }
        }
        else if (myNum == TutorialInteraction.TutorialMonsters.skill && !tutorial.useSkill) return;

        base.OnDamaged(damage);
    }

    public override void Dead()
    {
        tutorial.monsters[(int)myNum].isKilled = true;
        gameObject.SetActive(false);
    }

    public void ChaseState(bool state)
    {
        isChase = state;

        if (state) SpeedReturn();
        else SpeedToZero();
    }
}
