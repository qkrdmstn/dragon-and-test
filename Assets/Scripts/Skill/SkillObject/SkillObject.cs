using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SkillObject : MonoBehaviour
{
    [Header("Base info")]
    public SkillStatusEffects statusEffect;
    public int damage;
    public Vector2 dir;

    public SkillObject()
    {
    }


    public virtual void Initialize(int _damage, Vector2 _dir,StatusEffect _statusEffect)
    {
        statusEffect = new SkillStatusEffects();
        damage = _damage;
        dir = _dir;
        statusEffect.status = _statusEffect;
    }

    public virtual void SkillAttack(MonsterBase2 monster)
    {

    }
}
