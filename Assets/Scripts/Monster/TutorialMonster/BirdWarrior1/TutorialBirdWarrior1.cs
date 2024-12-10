using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBirdWarrior1 : BirdWarrior1
{
    [Header("Tutorial----------")]
    public TutorialMonsters myNum;
    public TutorialInteraction.MonsterState myState;
    TutorialInteraction tutorial;

    public int myHwatuNum;
    bool isInstantiated = false;

    protected override void Awake()
    {
        tutorial = FindObjectOfType<TutorialInteraction>();
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        myState = tutorial.monsters[(int)myNum];
    }

    public override void InitStates()
    {
        base.InitStates();

        spawnState = new MonsterSpawnStateBase(stateMachine, player, this);
        idleState = new MonsterIdleStateBase(stateMachine, player, this);
        deadState = new MonsterDeadState_TutorialWarrior1(stateMachine, player, this, tutorial);

        chaseState = new MonsterChaseState_BirdWarrior1(stateMachine, player, this);
        attackState = new MonsterAttackState_BirdWarrior1(stateMachine, player, this);
    }

    public override void OnDamaged(int damage)
    {
        if (myNum == TutorialMonsters.skill && !tutorial.useSkill) return;
        base.OnDamaged(damage);
    }

    public override void HwatuObjectDrop()
    {
        if (isInstantiated) return;

        isInstantiated = true;
        GameObject hwatuObj = Instantiate(ItemManager.instance.hwatuItemObj, this.transform.position, Quaternion.identity);
        hwatuObj.GetComponent<HwatuItemObject>().hwatuData = ItemManager.instance.hwatuDatas[myHwatuNum];
    }
}
