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
    public string[] _selectType;

    public DialogData(int eventOrder, string eventName, string npcName, string dialogue, bool isSelect, string selectType, int price)
    {
        _eventOrder = eventOrder;
        _eventName = eventName;
        _npcName = npcName;
        if (dialogue.Contains("dd")) dialogue = dialogue.Replace("dd", price.ToString());
        _dialogue = dialogue;
        _isSelect = isSelect;

        if (selectType != null)
            _selectType = selectType.Split(',');
        else
            _selectType = null;
    }
    public DialogData(int eventOrder, string eventName, string npcName, string dialogue, bool isSelect, string selectType)
    {
        _eventOrder = eventOrder;
        _eventName = eventName;
        _npcName = npcName;
        _dialogue = dialogue;
        _isSelect = isSelect;

        if (selectType != null)
            _selectType = selectType.Split(',');
        else
            _selectType = null;
    }
}
