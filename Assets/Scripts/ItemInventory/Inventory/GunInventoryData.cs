using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class GunInventoryData : MonoBehaviour
{
    public static GunInventoryData instance;

    public List<InventoryItem> gunItems;
    public Dictionary<ItemData, InventoryItem> gunDictionary;

    [Header("GunInventory UI")]
    [SerializeField] private Transform gunSlotParent;
    private ItemSlotUI[] gunItemSlot;


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
        gunItems = new List<InventoryItem>();
        gunDictionary = new Dictionary<ItemData, InventoryItem>();

        gunItemSlot = gunSlotParent.GetComponentsInChildren<ItemSlotUI>();
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

    private void UpdateSlotUI()
    {
        for (int i = 0; i < gunItems.Count; i++)
        {
            gunItemSlot[i].UpdateSlot(gunItems[i]);
        }
    }

    //sawpinventory
}
