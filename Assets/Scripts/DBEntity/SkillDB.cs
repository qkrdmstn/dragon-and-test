using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillDB 
{
    //public SeotdaHwatuCombination synergyCode;
    public string synergyCode;
    public string synergyName;
    public string info;
    public int damage;
    public float coolTime;
    public float range;
    public float force;
    public float duration;
    public float speed;
    public float period;
    public float probability;
    public float growCoefficient;

    public SkillDB(string synergyCode, string synergyName, string info, int damage, float coolTime, float range, float force, float duration, float speed, float period)
    {
        this.synergyCode = synergyCode;
        this.synergyName = synergyName;
        this.info = info;
        this.damage = damage;
        this.coolTime = coolTime;
        this.range = range;
        this.force = force;
        this.duration = duration;
        this.speed = speed;
        this.period = period;
    }

    public SeotdaHwatuCombination TransStringToEnum()
    {
        return (SeotdaHwatuCombination)Enum.Parse(typeof(SeotdaHwatuCombination), synergyCode);
    }
}
