using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialNear : MonsterNear
{
    [Header("Tutorial----------")]
    public TutorialMonsters myNum;
    public TutorialInteraction.MonsterState myState;
    TutorialInteraction tutorial;

    public int myHwatuNum;
    public override void Awake()
    {
        base.Awake();
        tutorial = FindObjectOfType<TutorialInteraction>();
    }
    public override void Start()
    {
        base.Start();
        myState = tutorial.monsters[(int)myNum];
    }

    public override void Update()
    {
        base.Update();
    }
    public override void OnDamaged(int damage)
    {
        if (myNum == TutorialMonsters.skill && !tutorial.useSkill) return;

        base.OnDamaged(damage);
    }

    public override void Dead()
    {
        if(myHwatuNum > 0)
            HwatuObjectDrop();

        tutorial.monsters[(int)myNum].isKilled = true;
        SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.Dead);

        gameObject.SetActive(false);
    }

    bool isInstantiated = false;
    public override void HwatuObjectDrop()
    {
        if (isInstantiated) return;

        isInstantiated = true;
        GameObject hwatuObj = Instantiate(SkillManager.instance.hwatuItemObj, this.transform.position, Quaternion.identity);
        hwatuObj.GetComponent<HwatuItemObject>().hwatuData = SkillManager.instance.hwatuData[myHwatuNum];
    }
}
