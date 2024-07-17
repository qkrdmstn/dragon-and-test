using System;

[Serializable]
public class PlayerData
{
    public int totalPlayTime;   // mins->parse필요
    public Date date;
    public string chapterName;

    int playerHP;
    int playerMP;
    int[] skills;
    int[] hwatus;
    int money;

    int curGun;
    int curBullets;
    int curShields;

    public PlayerData()
    {
        skills = new int[2];
        hwatus = new int[10];
    }
}

public struct Date
{
    int year;
    int month;
    int day;
    int hour;
    int min;

    public Date(int _year, int _month, int _day, int _hour, int _min)
    {
        year = _year;
        month = _month;
        day = _day;
        hour = _hour;
        min = _min;
    }
}

