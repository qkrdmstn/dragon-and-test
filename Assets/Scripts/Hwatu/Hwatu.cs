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
    KK9, KK8, KK7, KK6, KK5, KK4, KK3, KK2, KK1, KK0,
    AHES74, TTCatch73, MTGR94, PT94, blank
}
#endregion

[System.Serializable]
public class Hwatu
{
    public SeotdaHwatuName type;
    public HwatuMonth month;

    public static SeotdaHwatuCombination GetHwatuCombination(Hwatu card1, Hwatu card2)
    {
        if (card1 == null || card2 == null)
            return SeotdaHwatuCombination.blank;

        int[] months = new int[2] { (int)card1.month + 1, (int)card2.month + 1 };
        SeotdaHwatuName[] types = new SeotdaHwatuName[2] { card1.type, card2.type };

        //TT Set
        if (months[0] == months[1])
        {
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
                    break;
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
            return SeotdaHwatuCombination.PT94;
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
                    break;
            }
        }

        return SeotdaHwatuCombination.KK0;
    }

}
