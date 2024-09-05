
using Unity.VisualScripting;

[System.Serializable]
public class TutorialDBEntity
{
    public int eventOrder;
    public int sequence;
    public string dialogue;
    public bool isInteraction;

    public TutorialDBEntity(int _eventOrder, int _sequence, string _dialogue, bool _isInteraction)
    {
        eventOrder = _eventOrder;
        sequence = _sequence;
        dialogue = _dialogue;
        isInteraction = _isInteraction;
    }
}


