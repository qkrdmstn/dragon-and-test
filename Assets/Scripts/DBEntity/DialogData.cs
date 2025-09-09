using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DialogData
{
    public int _eventOrder;
    public string _eventName;
    public string _npcName;
    public string _dialogue;

    public bool _isSelect;
    public int _imageIdx;
    public int _cameraEffectNum;
    public string[] _selectType;

    public DialogData(int eventOrder, string eventName, string npcName, string dialogue, bool isSelect, string selectType, int imageIdx, int cameraEffectNum)
    {
        _eventOrder = eventOrder;
        _eventName = eventName;
        _npcName = npcName;
        _dialogue = dialogue;
        _isSelect = isSelect;
        _imageIdx = imageIdx;
        _cameraEffectNum = cameraEffectNum;

        if (selectType != null)
            _selectType = selectType.Split(',');
        else
            _selectType = null;
    }
}
