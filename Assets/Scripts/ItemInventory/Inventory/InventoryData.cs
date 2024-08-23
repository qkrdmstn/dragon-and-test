using System.Collections.Generic;
using UnityEngine;

public class InventoryData : MonoBehaviour
{
    public static InventoryData instance;

    public List<InventoryItem> gunItems;
    public Dictionary<ItemData, InventoryItem> gunDictionary;

    public InventoryItem armorItem;

    [Header("Inventory UI")]
    [SerializeField] private Transform gunSlotParent;
    [SerializeField] private Transform amorSlotParent;
    private ItemSlotUI[] gunItemSlot;
    private ItemSlotUI amorItemSlot;

    private void Awake()
    {
        if (instance == null)
        { //생성 전이면
            instance = this; //생성

            gunDictionary = new Dictionary<ItemData, InventoryItem>();

            gunItemSlot = gunSlotParent.GetComponentsInChildren<ItemSlotUI>(true);
            amorItemSlot = amorSlotParent.GetComponentInChildren<ItemSlotUI>(true);
        }
        else if (instance != this)
        { //이미 생성되어 있으면
            Destroy(this.gameObject); //새로만든거 삭제
        }

        DontDestroyOnLoad(this.gameObject); //씬이 넘어가도 오브젝트 유지
    }

    private void Start()
    {
        gunItems = new List<InventoryItem>();
    }

    public void AddGunItem(ItemData _gunItem)
    {
        //인벤토리에 없는 경우에만 추가. (무기 중복 X)
        if (!gunDictionary.TryGetValue(_gunItem, out InventoryItem value))
        {
            InventoryItem newItem = new InventoryItem(_gunItem);
            gunItems.Add(newItem);
            gunDictionary.Add(_gunItem, newItem);
        }
        UpdateSlotUI();
    }

    public void AddArmorItem(ItemData _armorItem)
    {
        armorItem = new InventoryItem(_armorItem);
        UpdateSlotUI();
    }

    private void UpdateSlotUI()
    {
        for(int i=0; i<gunItems.Count; i++)
        {
            gunItemSlot[i].UpdateSlot(gunItems[i]);
        }

        if(armorItem.data != null)
            amorItemSlot.UpdateSlot(armorItem);
    }
}
