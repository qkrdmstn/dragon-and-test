using System.Collections.Generic;
using UnityEngine;

public class InventoryData : MonoBehaviour
{
    public static InventoryData instance;
    public InventoryItem armorItem;

    [Header("Inventory UI")]
    [SerializeField] private Transform amorSlotParent;
    private ItemSlotUI amorItemSlot;

    private void Awake()
    {
        if (instance == null)
        { //생성 전이면
            instance = this; //생성
            amorItemSlot = amorSlotParent.GetComponentInChildren<ItemSlotUI>(true);
        }
        else if (instance != this)
        { //이미 생성되어 있으면
            Destroy(this.gameObject); //새로만든거 삭제
        }

        DontDestroyOnLoad(this.gameObject); //씬이 넘어가도 오브젝트 유지
    }

    public void AddArmorItem(ItemData _armorItem)
    {
        armorItem = new InventoryItem(_armorItem);
        UpdateSlotUI();
    }

    private void UpdateSlotUI()
    {
        if(armorItem.data != null)
            amorItemSlot.UpdateSlot(armorItem);
    }
}
