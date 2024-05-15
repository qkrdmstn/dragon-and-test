using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public struct CombinationInfo
{
    string name;
    string synergyDesc;

    HwatuData[] imgs;

    public CombinationInfo(string _name, string _snergyDesc, HwatuData[] _datas)
    {
        name = _name;
        synergyDesc = _snergyDesc;

        imgs = new HwatuData[2];
        imgs[0] = _datas[0];
        imgs[1] = _datas[1];
    }
}

public class JokboUIGroup : UIGroup {

    void InitDesc()
    {

    }
}
