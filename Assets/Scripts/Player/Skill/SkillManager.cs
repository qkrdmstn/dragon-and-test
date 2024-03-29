using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager: MonoBehaviour
{
    public Player player;

    private void Update()
    {
        
    }
    public void SkillEffect(HwatuCombination result)
    {
        Debug.Log(result.ToString());
        switch (result)
        {
            case HwatuCombination.GTT38:

                break;
            case HwatuCombination.GTT18:

                break;
            case HwatuCombination.GTT13:

                break;
            case HwatuCombination.JTT:

                break;
            case HwatuCombination.TT9:

                break;
            case HwatuCombination.TT8:

                break;
            case HwatuCombination.TT7:

                break;
            case HwatuCombination.TT6:

                break;
            case HwatuCombination.TT5:

                break;
            case HwatuCombination.TT4:

                break;
            case HwatuCombination.TT3:

                break;
            case HwatuCombination.TT2:

                break;
            case HwatuCombination.TT1:

                break;
            case HwatuCombination.AL12:

                break;
            case HwatuCombination.DS14:

                break;
            case HwatuCombination.GPP19:

                break;
            case HwatuCombination.JPP110:

                break;
            case HwatuCombination.JS410:

                break;
            case HwatuCombination.SR46:

                break;
            case HwatuCombination.KK9:

                break;
            case HwatuCombination.KK8:

                break;
            case HwatuCombination.KK7:

                break;
            case HwatuCombination.KK6:

                break;
            case HwatuCombination.KK5:

                break;
            case HwatuCombination.KK4:

                break;
            case HwatuCombination.KK3:

                break;
            case HwatuCombination.KK2:

                break;
            case HwatuCombination.KK1:

                break;
            case HwatuCombination.KK0:
                break;
            case HwatuCombination.AHES74:

                break;
            case HwatuCombination.TTCatch73:

                break;
            case HwatuCombination.MTGR94:

                break;
            case HwatuCombination.PT94:

                break;
        }
    }

    private void MaxHPControl(int increase)
    {
        player.curHP += increase;
        player.maxHP += increase;
    }

    private void SpeedControl(int increase)
    {
        player.curHP += increase;
        player.maxHP += increase;
    }

    private void AttackSpeedControl(int increase)
    {
        player.curHP += increase;
        player.maxHP += increase;
    }
}
