using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public float totalPlayTime;   // mins->parse필요
    public Date date;
    public string chapterName;

    public int playerHP;
    public int playerMP;
    public Dictionary<ActiveSkillSlot, ActiveSlotData> activeSkill;
    public Dictionary<SeotdaHwatuCombination, int> passiveSkill;
    public List<HwatuData> hwatus;

    public int money;
    public GunData curGun;
    public int curShieldCnt;
    public bool isClearTutorial;

    public PlayerData(float originPlayTime)
    {
        date = new Date(
            DateTime.Now.ToString("yyyy"),
            DateTime.Now.ToString("MM"),
            DateTime.Now.ToString("dd"),
            DateTime.Now.ToString("HH"),
            DateTime.Now.ToString("mm")
        );

        totalPlayTime = originPlayTime + Time.realtimeSinceStartup;   // sec이므로 시분으로 바꿔야함
        chapterName = "Chapter 01. 이무기 마을";

        playerHP = Player.instance.GetCurHP();
        money = Player.instance.GetCurMoney();
        playerMP = 0; // 아직 미개발

        activeSkill = SkillManager.instance.active.refSkill;
        passiveSkill = SkillManager.instance.passive.refSkill;

        hwatus = SkillManager.instance.materialHwatuDataList == null ? new List<HwatuData>() : SkillManager.instance.materialHwatuDataList;

        curGun = GunManager.instance.currentGun.GetComponent<Gun>().initData;
        curShieldCnt = Player.instance.GetCurShield();

        isClearTutorial = Player.instance.isClearTutorial;
    }
}

[Serializable]
public struct Date
{
    public string year;
    public string month;
    public string day;
    public string hour;
    public string min;

    public Date(string _year,
                string _month,
                string _day,
                string _hour,
                string _min)
    {
        year = _year;
        month = _month;
        day = _day;
        hour = _hour;
        min = _min;
    }
}

