using UnityEngine;

public enum ItemType
{
    Material,
    Gun,
    Armor,
    DragonHwatu
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public string itemInfo;
    public Sprite icon;
}