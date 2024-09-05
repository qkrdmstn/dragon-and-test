using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Enum
public enum HwatuMonth
{
    Jan, Feb, Mar, Apr, May, Jun,
    Jul, Aug, Sep, Oct, Nov, Dec
}

public enum HwatuType
{
    G, KK, D, SSP, P
}

public enum SeotdaHwatuName
{
    JanCrane, JanPine,
    FebBird, FebPrunus,
    MarCherryLight, MarCherry,
    AprCuckoo, AprWisteria,
    MayBridge, MayIris,
    JunButterfly, JunPeony,
    JulBoar, JulLespedeza,
    AugMoon, AugGoose,
    SepSakajuki, SepChrysanthemum,
    OctDeer, OctFoliage
}

public enum SeotdaHwatuCombination
{
    GTT38, GTT18, GTT13,
    JTT, 
    TT9, TT8, TT7, TT6, TT5, TT4, TT3, TT2, TT1,
    AL12, DS14, GPP19, JPP110, JS410, SR46,
    AHES74, TTCatch73, MTGR94,
    KK9, KK8, KK7, KK6, KK5, KK4, KK3, KK2, KK1, KK0,
    blank
}
#endregion

[System.Serializable]
public class Hwatu
{
    public SeotdaHwatuName type;
    public HwatuMonth month;
    public bool isMain;

    public static SeotdaHwatuCombination GetHwatuCombination(Hwatu card1, Hwatu card2)
    {
        if (card1 == null || card2 == null)
            return SeotdaHwatuCombination.blank;

        int[] months = new int[2] { (int)card1.month + 1, (int)card2.month + 1 };
        SeotdaHwatuName[] types = new SeotdaHwatuName[2] { card1.type, card2.type };

        //TT Set
        if (months[0] == months[1])
        {
            if (types[0] == types[1])
                return SeotdaHwatuCombination.blank;

            switch (months[0])
            {
                case 10:
                    return SeotdaHwatuCombination.JTT;
                case 9:
                    return SeotdaHwatuCombination.TT9;
                case 8:
                    return SeotdaHwatuCombination.TT8;
                case 7:
                    return SeotdaHwatuCombination.TT7;
                case 6:
                    return SeotdaHwatuCombination.TT6;
                case 5:
                    return SeotdaHwatuCombination.TT5;
                case 4:
                    return SeotdaHwatuCombination.TT4;
                case 3:
                    return SeotdaHwatuCombination.TT3;
                case 2:
                    return SeotdaHwatuCombination.TT2;
                case 1:
                    return SeotdaHwatuCombination.TT1;
                default:
                    return SeotdaHwatuCombination.blank;
                    
            }
        }
        else if (Array.Exists(types, x => x == SeotdaHwatuName.MarCherryLight) && Array.Exists(types, x => x == SeotdaHwatuName.AugMoon))
            return SeotdaHwatuCombination.GTT38;
        else if (Array.Exists(types, x => x == SeotdaHwatuName.SepSakajuki) && Array.Exists(types, x => x == SeotdaHwatuName.AprCuckoo))
            return SeotdaHwatuCombination.MTGR94;
        else if (Array.Exists(types, x => x == SeotdaHwatuName.JanCrane) && Array.Exists(types, x => x == SeotdaHwatuName.AugMoon))
            return SeotdaHwatuCombination.GTT18;
        else if (Array.Exists(types, x => x == SeotdaHwatuName.JanCrane) && Array.Exists(types, x => x == SeotdaHwatuName.MarCherryLight))
            return SeotdaHwatuCombination.GTT13;
        else if (Array.Exists(types, x => x == SeotdaHwatuName.JulBoar) && Array.Exists(types, x => x == SeotdaHwatuName.AprCuckoo))
            return SeotdaHwatuCombination.AHES74;
        else if (Array.Exists(months, x => x == 9) && Array.Exists(months, x => x == 4))
            return SeotdaHwatuCombination.MTGR94; //PT94
        else if (Array.Exists(months, x => x == 7) && Array.Exists(months, x => x == 3))
            return SeotdaHwatuCombination.TTCatch73;
        else if (Array.Exists(months, x => x == 1) && Array.Exists(months, x => x == 2))
            return SeotdaHwatuCombination.AL12;
        else if (Array.Exists(months, x => x == 1) && Array.Exists(months, x => x == 4))
            return SeotdaHwatuCombination.DS14;
        else if (Array.Exists(months, x => x == 1) && Array.Exists(months, x => x == 9))
            return SeotdaHwatuCombination.GPP19;
        else if (Array.Exists(months, x => x == 1) && Array.Exists(months, x => x == 10))
            return SeotdaHwatuCombination.JPP110;
        else if (Array.Exists(months, x => x == 4) && Array.Exists(months, x => x == 10))
            return SeotdaHwatuCombination.JS410;
        else if (Array.Exists(months, x => x == 4) && Array.Exists(months, x => x == 6))
            return SeotdaHwatuCombination.SR46;
        else  //KK Set
        {
            int sum = months[0] + months[1];
            sum %= 10;

            switch (sum)
            {
                case 9:
                    return SeotdaHwatuCombination.KK9;
                case 8:
                    return SeotdaHwatuCombination.KK8;
                case 7:
                    return SeotdaHwatuCombination.KK7;
                case 6:
                    return SeotdaHwatuCombination.KK6;
                case 5:
                    return SeotdaHwatuCombination.KK5;
                case 4:
                    return SeotdaHwatuCombination.KK4;
                case 3:
                    return SeotdaHwatuCombination.KK3;
                case 2:
                    return SeotdaHwatuCombination.KK2;
                case 1:
                    return SeotdaHwatuCombination.KK1;
                case 0:
                    return SeotdaHwatuCombination.KK0;
                default:
                    return SeotdaHwatuCombination.blank;
            }
        }
    }

    public static SeotdaHwatuName[] GetHwatuCombination(SeotdaHwatuCombination combination)
    {   // ??? ?? ??? ?? ??? ???? ??
        //Hwatu[] hwatus = new Hwatu[2];

        SeotdaHwatuName[] hwatus = new SeotdaHwatuName[2];
        switch (combination)
        {
            case SeotdaHwatuCombination.GTT38:
                hwatus[0] = SeotdaHwatuName.MarCherryLight;
                hwatus[1] = SeotdaHwatuName.AugMoon;
                break;
            case SeotdaHwatuCombination.GTT18:
                hwatus[0] = SeotdaHwatuName.JanCrane;
                hwatus[1] = SeotdaHwatuName.AugMoon;
                break;
            case SeotdaHwatuCombination.GTT13:
                hwatus[0] = SeotdaHwatuName.JanCrane;
                hwatus[1] = SeotdaHwatuName.MarCherryLight;
                break;
            case SeotdaHwatuCombination.JTT:    //10
                hwatus[0] = SeotdaHwatuName.OctDeer;
                hwatus[1] = SeotdaHwatuName.OctFoliage;
                break;
            case SeotdaHwatuCombination.TT9:
                hwatus[0] = SeotdaHwatuName.SepSakajuki;
                hwatus[1] = SeotdaHwatuName.SepChrysanthemum;
                break;
            case SeotdaHwatuCombination.TT8:
                hwatus[0] = SeotdaHwatuName.AugMoon;
                hwatus[1] = SeotdaHwatuName.AugGoose;
                break;
            case SeotdaHwatuCombination.TT7:
                hwatus[0] = SeotdaHwatuName.JulBoar;
                hwatus[1] = SeotdaHwatuName.JulLespedeza;
                break;
            case SeotdaHwatuCombination.TT6:
                hwatus[0] = SeotdaHwatuName.JunButterfly;
                hwatus[1] = SeotdaHwatuName.JunPeony;
                break;
            case SeotdaHwatuCombination.TT5:
                hwatus[0] = SeotdaHwatuName.MayIris;
                hwatus[1] = SeotdaHwatuName.MayBridge;
                break;
            case SeotdaHwatuCombination.TT4:
                hwatus[0] = SeotdaHwatuName.AprWisteria;
                hwatus[1] = SeotdaHwatuName.AprCuckoo;
                break;
            case SeotdaHwatuCombination.TT3:
                hwatus[0] = SeotdaHwatuName.MarCherryLight;
                hwatus[1] = SeotdaHwatuName.MarCherry;
                break;
            case SeotdaHwatuCombination.TT2:
                hwatus[0] = SeotdaHwatuName.FebPrunus;
                hwatus[1] = SeotdaHwatuName.FebBird;
                break;
            case SeotdaHwatuCombination.TT1:
                hwatus[0] = SeotdaHwatuName.JanPine;
                hwatus[1] = SeotdaHwatuName.JanCrane;
                break;
            case SeotdaHwatuCombination.AL12:
                hwatus[0] = SeotdaHwatuName.JanCrane;
                hwatus[1] = SeotdaHwatuName.FebBird;
                break;
            case SeotdaHwatuCombination.DS14:
                hwatus[0] = SeotdaHwatuName.JanCrane;
                hwatus[1] = SeotdaHwatuName.AprCuckoo;
                break;
            case SeotdaHwatuCombination.GPP19:
                hwatus[0] = SeotdaHwatuName.JanCrane;
                hwatus[1] = SeotdaHwatuName.SepSakajuki;
                break;
            case SeotdaHwatuCombination.JPP110:
                hwatus[0] = SeotdaHwatuName.JanCrane;
                hwatus[1] = SeotdaHwatuName.OctDeer;
                break;
            case SeotdaHwatuCombination.JS410:
                hwatus[0] = SeotdaHwatuName.AprCuckoo;
                hwatus[1] = SeotdaHwatuName.OctDeer;
                break;
            case SeotdaHwatuCombination.SR46:
                hwatus[0] = SeotdaHwatuName.AprCuckoo;
                hwatus[1] = SeotdaHwatuName.JunButterfly;
                break;
            case SeotdaHwatuCombination.MTGR94:
                hwatus[0] = SeotdaHwatuName.AprCuckoo;
                hwatus[1] = SeotdaHwatuName.SepSakajuki;
                break;
            case SeotdaHwatuCombination.TTCatch73:
                hwatus[0] = SeotdaHwatuName.JulBoar;
                hwatus[1] = SeotdaHwatuName.MarCherryLight;
                break;
            case SeotdaHwatuCombination.AHES74:
                hwatus[0] = SeotdaHwatuName.JulBoar;
                hwatus[1] = SeotdaHwatuName.AprCuckoo;
                break;
        }
        return hwatus;
    }

}
