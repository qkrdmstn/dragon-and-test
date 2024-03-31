using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Enum
public enum Month
{
    Jan, Feb, Mar, Apr, May,
    Jun, Jul, Aug, Sep, Oct
}

public enum HwatuType
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

public enum HwatuCombination
{
    GTT38, GTT18, GTT13,
    JTT, 
    TT9, TT8, TT7, TT6, TT5, TT4, TT3, TT2, TT1,
    AL12, DS14, GPP19, JPP110, JS410, SR46,
    KK9, KK8, KK7, KK6, KK5, KK4, KK3, KK2, KK1, KK0,
    AHES74, TTCatch73, MTGR94, PT94,
}
#endregion

[System.Serializable]
public class Hwatu
{
    public HwatuType type;
    public Month month;

    public static HwatuCombination GetHwatuCombination(Hwatu card1, Hwatu card2)
    {
        int[] months = new int[2] { (int)card1.month + 1, (int)card2.month + 1 };
        HwatuType[] types = new HwatuType[2] { card1.type, card2.type };

        //TT Set
        if (months[0] == months[1])
        {
            switch (months[0])
            {
                case 10:
                    return HwatuCombination.JTT;
                case 9:
                    return HwatuCombination.TT9;
                case 8:
                    return HwatuCombination.TT8;
                case 7:
                    return HwatuCombination.TT7;
                case 6:
                    return HwatuCombination.TT6;
                case 5:
                    return HwatuCombination.TT5;
                case 4:
                    return HwatuCombination.TT4;
                case 3:
                    return HwatuCombination.TT3;
                case 2:
                    return HwatuCombination.TT2;
                case 1:
                    return HwatuCombination.TT1;
                default:
                    break;
            }
        }
        else if (Array.Exists(types, x => x == HwatuType.MarCherryLight) && Array.Exists(types, x => x == HwatuType.AugMoon))
            return HwatuCombination.GTT38;
        else if (Array.Exists(types, x => x == HwatuType.SepSakajuki) && Array.Exists(types, x => x == HwatuType.AprCuckoo))
            return HwatuCombination.MTGR94;
        else if (Array.Exists(types, x => x == HwatuType.JanCrane) && Array.Exists(types, x => x == HwatuType.AugMoon))
            return HwatuCombination.GTT18;
        else if (Array.Exists(types, x => x == HwatuType.JanCrane) && Array.Exists(types, x => x == HwatuType.MarCherryLight))
            return HwatuCombination.GTT13;
        else if (Array.Exists(types, x => x == HwatuType.JulBoar) && Array.Exists(types, x => x == HwatuType.AprCuckoo))
            return HwatuCombination.AHES74;
        else if (Array.Exists(months, x => x == 9) && Array.Exists(months, x => x == 4))
            return HwatuCombination.PT94;
        else if (Array.Exists(months, x => x == 7) && Array.Exists(months, x => x == 3))
            return HwatuCombination.TTCatch73;
        else if (Array.Exists(months, x => x == 1) && Array.Exists(months, x => x == 2))
            return HwatuCombination.AL12;
        else if (Array.Exists(months, x => x == 1) && Array.Exists(months, x => x == 4))
            return HwatuCombination.DS14;
        else if (Array.Exists(months, x => x == 1) && Array.Exists(months, x => x == 9))
            return HwatuCombination.GPP19;
        else if (Array.Exists(months, x => x == 1) && Array.Exists(months, x => x == 10))
            return HwatuCombination.JPP110;
        else if (Array.Exists(months, x => x == 4) && Array.Exists(months, x => x == 10))
            return HwatuCombination.JS410;
        else if (Array.Exists(months, x => x == 4) && Array.Exists(months, x => x == 6))
            return HwatuCombination.SR46;
        else  //KK Set
        {
            int sum = months[0] + months[1];
            sum %= 10;

            switch (sum)
            {
                case 9:
                    return HwatuCombination.KK9;
                case 8:
                    return HwatuCombination.KK8;
                case 7:
                    return HwatuCombination.KK7;
                case 6:
                    return HwatuCombination.KK6;
                case 5:
                    return HwatuCombination.KK5;
                case 4:
                    return HwatuCombination.KK4;
                case 3:
                    return HwatuCombination.KK3;
                case 2:
                    return HwatuCombination.KK2;
                case 1:
                    return HwatuCombination.KK1;
                case 0:
                    return HwatuCombination.KK0;
                default:
                    break;
            }
        }

        return HwatuCombination.KK0;
    }
}
