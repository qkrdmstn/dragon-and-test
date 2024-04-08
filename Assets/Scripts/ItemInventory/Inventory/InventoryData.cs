using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class InventoryData : MonoBehaviour
{
    public static InventoryData instance;

    public List<InventoryItem> inventoryItems;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    public List<InventoryItem> gunItems;
    public Dictionary<ItemData, InventoryItem> gunDictionary;

    public InventoryItem amorItem;
    public Dictionary<ItemData, InventoryItem> amorDictionary; 

    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform gunSlotParent;
    [SerializeField] private Transform amorSlotParent;
    private ItemSlotUI[] inventoryItemSlot;
    private ItemSlotUI[] gunItemSlot;
    private ItemSlotUI amorItemSlot;

    private void Awake()
    {
        if (instance == null)
        { //생성 전이면
            instance = this; //생성
        }
        else if (instance != this)
        { //이미 생성되어 있으면
            Destroy(this.gameObject); //새로만든거 삭제
        }

        DontDestroyOnLoad(this.gameObject); //씬이 넘어가도 오브젝트 유지
    }

    private void Start()
    {
        inventoryItems = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        gunItems = new List<InventoryItem>();
        gunDictionary = new Dictionary<ItemData, InventoryItem>();

        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<ItemSlotUI>();
        gunItemSlot = gunSlotParent.GetComponentsInChildren<ItemSlotUI>();
        amorItemSlot = amorSlotParent.GetComponentInChildren<ItemSlotUI>();
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

    //public void AddAmorItem(ItemData _amorItem)
    //{
    //    amorItem = new InventoryItem(_amorItem);
    //    amorDictionary.Clear();
    //    amorDictionary.Add(_amorItem, amorItem);
    //}

    private void UpdateSlotUI()
    {
        for(int i=0; i<inventoryItems.Count; i++)
        {
            inventoryItemSlot[i].UpdateSlot(inventoryItems[i]);
        }

        for(int i=0; i<gunItems.Count; i++)
        {
            gunItemSlot[i].UpdateSlot(gunItems[i]);
        }

        if(amorItem.data != null)
            amorItemSlot.UpdateSlot(amorItem);
    }

    public void AddItem(ItemData _item)
    {
        //이미 인벤토리에 존재할 경우, Stack 크기만 증가
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventoryItems.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }

        UpdateSlotUI();
    }
    
    public void RemoveItem(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if(value.stackSize <= 1)
            {
                inventoryItems.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            else
                value.RemoveStack();
        }
        UpdateSlotUI();
    }
}
