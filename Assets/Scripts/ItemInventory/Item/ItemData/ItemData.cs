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
    [TextArea] public string itemInfo;
    public int price;
    public Sprite icon;
}
