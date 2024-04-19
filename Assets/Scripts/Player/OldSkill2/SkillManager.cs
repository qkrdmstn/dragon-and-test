using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager: MonoBehaviour
{
    public Player player;

    private void Update()
    {
        
    }
    public void SkillEffect(SeotdaHwatuCombination result)
    {
        Debug.Log(result.ToString());
        switch (result)
        {
            case SeotdaHwatuCombination.GTT38:

                break;
            case SeotdaHwatuCombination.GTT18:

                break;
            case SeotdaHwatuCombination.GTT13:

                break;
            case SeotdaHwatuCombination.JTT:

                break;
            case SeotdaHwatuCombination.TT9:

                break;
            case SeotdaHwatuCombination.TT8:

                break;
            case SeotdaHwatuCombination.TT7:

                break;
            case SeotdaHwatuCombination.TT6:

                break;
            case SeotdaHwatuCombination.TT5:

                break;
            case SeotdaHwatuCombination.TT4:

                break;
            case SeotdaHwatuCombination.TT3:

                break;
            case SeotdaHwatuCombination.TT2:

                break;
            case SeotdaHwatuCombination.TT1:

                break;
            case SeotdaHwatuCombination.AL12:

                break;
            case SeotdaHwatuCombination.DS14:

                break;
            case SeotdaHwatuCombination.GPP19:

                break;
            case SeotdaHwatuCombination.JPP110:

                break;
            case SeotdaHwatuCombination.JS410:

                break;
            case SeotdaHwatuCombination.SR46:

                break;
            case SeotdaHwatuCombination.KK9:

                break;
            case SeotdaHwatuCombination.KK8:

                break;
            case SeotdaHwatuCombination.KK7:

                break;
            case SeotdaHwatuCombination.KK6:

                break;
            case SeotdaHwatuCombination.KK5:

                break;
            case SeotdaHwatuCombination.KK4:

                break;
            case SeotdaHwatuCombination.KK3:

                break;
            case SeotdaHwatuCombination.KK2:

                break;
            case SeotdaHwatuCombination.KK1:

                break;
            case SeotdaHwatuCombination.KK0:
                break;
            case SeotdaHwatuCombination.AHES74:

                break;
            case SeotdaHwatuCombination.TTCatch73:

                break;
            case SeotdaHwatuCombination.MTGR94:

                break;
            case SeotdaHwatuCombination.PT94:

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
