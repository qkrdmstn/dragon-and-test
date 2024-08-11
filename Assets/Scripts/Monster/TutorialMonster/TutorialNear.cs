using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialNear : MonsterNear
{
    public int myHwatuNum;
    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
        base.Start();
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

    public override void HwatuObjectDrop()
    {
        GameObject hwatuObj = Instantiate(SkillManager.instance.hwatuItemObj, this.transform.position, Quaternion.identity);
        hwatuObj.GetComponent<HwatuItemObject>().hwatuData = SkillManager.instance.hwatuData[myHwatuNum];
    }
}
