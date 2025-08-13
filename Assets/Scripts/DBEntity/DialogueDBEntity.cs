
using System.Diagnostics;

[System.Serializable]
public class DialogueDBEntity
{
    public int eventOrder;
    public string eventName;
    public string npcName;
    public string dialogue;
    public bool isSelect;
    public string selectType;
    public int imageIdx;

    public DialogueDBEntity(int _eventOrder, string _eventName, string _npcName, string _dialogue, bool _isSelect, string _selectType, int _imageIdx)
    {
        eventOrder = _eventOrder;
        eventName = _eventName;
        npcName = _npcName;
        dialogue = _dialogue;
        isSelect = _isSelect;
        selectType = _selectType;
        imageIdx = _imageIdx;
    }

    public DialogueDBEntity(int _eventOrder, string _eventName, string _npcName, string _dialogue, bool _isSelect, int _imageIdx)
    {
        eventOrder = _eventOrder;
        eventName = _eventName;
        npcName = _npcName;
        dialogue = _dialogue;
        isSelect = _isSelect;
        imageIdx = _imageIdx;
    }
}
