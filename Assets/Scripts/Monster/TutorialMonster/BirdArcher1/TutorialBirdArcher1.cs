
using UnityEngine;

public class TutorialBirdArcher1 : BirdArcher1
{
    [Header("Tutorial----------")]
    public TutorialMonsters myNum;
    public TutorialInteraction tutorial;
    public bool isAttackable = false;

    protected override void Awake()
    {
        tutorial = FindObjectOfType<TutorialInteraction>();
        base.Awake();
    }

    public override void InitStates()
    {
        base.InitStates();

        spawnState = new MonsterSpawnStateBase(stateMachine, player, this);
        idleState = new MonsterIdleState_TutorialArcher1(stateMachine, player, this);
        deadState = new MonsterDeadState_TutorialArcher1(stateMachine, player, this, tutorial);

        chaseState = new MonsterChaseState_BirdArcher1(stateMachine, player, this);
        attackState = new MonsterAttackState_BirdArcher1(stateMachine, player, this);
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
}
