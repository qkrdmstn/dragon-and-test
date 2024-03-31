using UnityEngine;

public enum ItemType
{
    Material,
    Gun,
    DragonHwatu
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
}
