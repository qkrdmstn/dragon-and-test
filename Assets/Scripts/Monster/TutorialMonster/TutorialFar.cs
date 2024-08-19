
using UnityEngine;

public class TutorialFar : MonsterFar
{
    [Header("Tutorial----------")]
    public Tutorial.TutorialMonsters myNum;
    public Tutorial tutorial;

    public override void Awake()
    {
        base.Awake();
        tutorial = FindObjectOfType<Tutorial>();
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
        if (!isChase && myNum == Tutorial.TutorialMonsters.attack)
        {
            tutorial.isAttacked = true;
            return;
        }
        else if (myNum == Tutorial.TutorialMonsters.skill && !tutorial.useSkill) return;

        base.OnDamaged(damage);
    }

    public override void Dead()
    {
        tutorial.monsters[(int)myNum].isKilled = true;
        gameObject.SetActive(false);
    }

    public void ChangeChaseState(bool state)
    {
        isChase = state;
    }
}
