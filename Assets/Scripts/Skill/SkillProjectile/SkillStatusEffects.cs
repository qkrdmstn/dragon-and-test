using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffect
{
    None,
    Sokbak,
    Stun,
    Slow
}

public class SkillStatusEffects
{
    public StatusEffect status;

    public void ApplyStatusEffect(MonsterBase monster)
    {
        Debug.Log("StatusEffect");
        monster.EffectState();
        //Todo. MonsterBase 스크립트에 enum을 전달하는 status effect 적용 함수 호출

    }
}
