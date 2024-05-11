using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.UIElements;
using UnityEngine.UI;

public class InventoryData : MonoBehaviour
{
    public static InventoryData instance;

    public List<InventoryItem> materialItems;
    public Dictionary<ItemData, InventoryItem> materialDictionary;

    public List<InventoryItem> gunItems;
    public Dictionary<ItemData, InventoryItem> gunDictionary;

    public InventoryItem amorItem;
    public Dictionary<ItemData, InventoryItem> amorDictionary; 

    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform gunSlotParent;
    [SerializeField] private Transform amorSlotParent;
    private ItemSlotUI[] materialItemSlot;
    private ItemSlotUI[] gunItemSlot;
    private ItemSlotUI amorItemSlot;

    [Header("ItemUse UI")]
    [SerializeField] private GameObject itemUseUI;
    private Button[] itemUseBtns;
    
    private void Awake()
    {
        if (instance == null)
        { //생성 전이면
            instance = this; //생성

            gunDictionary = new Dictionary<ItemData, InventoryItem>();

            materialItemSlot = inventorySlotParent.GetComponentsInChildren<ItemSlotUI>(true);
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
        //Inventory UI Initialize
        materialItems = new List<InventoryItem>();
        materialDictionary = new Dictionary<ItemData, InventoryItem>();

        gunItems = new List<InventoryItem>();
        
        //ItemUse UI Initialize
        itemUseBtns = itemUseUI.GetComponentsInChildren<Button>();

        for (int i = 0; i < itemUseBtns.Length; i++)
            Debug.Log(itemUseBtns[i].name);
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
        for(int i=0; i< materialItemSlot.Length; i++)
        {
            materialItemSlot[i].ClearSlot();
        }

        for(int i=0; i<materialItems.Count; i++)
        {
            materialItemSlot[i].UpdateSlot(materialItems[i]);
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
        if (materialDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            materialItems.Add(newItem);
            materialDictionary.Add(_item, newItem);
        }

        UpdateSlotUI();
    }
    
    public void RemoveItem(ItemData _item)
    {
        if (materialDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if(value.stackSize <= 1)
            {
                materialItems.Remove(value);
                materialDictionary.Remove(_item);
            }
            else
                value.RemoveStack();
        }
        UpdateSlotUI();
    }

    public void ShowUseUI(MaterialItemData materialItemData)
    {
        itemUseUI.SetActive(true);
        //Yes
        itemUseBtns[0].onClick.AddListener(() => UseItemEffect(materialItemData));
        itemUseBtns[0].onClick.AddListener(() => RemoveItem(materialItemData));
        itemUseBtns[0].onClick.AddListener(CloseUseUI);

        //No
        itemUseBtns[1].onClick.AddListener(CloseUseUI);
    }

    public void CloseUseUI()
    {
        itemUseUI.SetActive(false);

        for (int i = 0; i < 2; i++)
        {
            itemUseBtns[i].onClick.RemoveAllListeners();
        }
    }

    public void UseItemEffect(MaterialItemData materialItemData)
    {

        materialItemData.ItemEffect();
    }
}
