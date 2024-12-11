using UnityEngine;

public class PickUpItemInteraction : Interaction
{
    GameObject interaction;
    ItemData itemData;

    public override void LoadEvent(InteractionData data)
    {
        Init();

        this.itemData = data.itemData;
        interaction = data.gameObject;

        switch (itemData.itemType)
        {
            case ItemType.Material:
                (itemData as EffectItemData)?.ItemEffect();
                // 인터렉션 데이터의 시퀀스 값에 더해질 effect value를 저장해서 넘겨주는 형태
                break;
            case ItemType.Gun:
                ItemManager.instance.gunController.addGunAction(itemData as GunItemData);
                break;
            case ItemType.Armor:
                (itemData as EffectItemData).ItemEffect();
                InventoryData.instance.AddArmorItem(itemData);
                break;
        }
        isDone = true;
        Destroy(interaction);
    }

    void Init()
    {
        isDone = false;
        interaction = null;
        itemData = null;
    }
}
