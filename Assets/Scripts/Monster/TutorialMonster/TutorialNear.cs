using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialNear : MonsterNear
{
    [Header("Tutorial----------")]
    public Tutorial.TutorialMonsters myNum;
    public Tutorial.MonsterState myState;
    Tutorial tutorial;

    public int myHwatuNum;
    public override void Awake()
    {
        base.Awake();
        tutorial = FindObjectOfType<Tutorial>();
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

    public override void Dead()
    {
        HwatuObjectDrop();
        Destroy(gameObject);
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
