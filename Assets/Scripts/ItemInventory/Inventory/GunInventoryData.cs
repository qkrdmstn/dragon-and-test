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

    public InventoryItem curGunItem;
    public int loadedBullet;
    public int magazineSize;

    [Header("GunInventory UI")]
    [SerializeField] private Transform gunSlotParent;
    private ItemSlotUI curGunItemSlot;

    //text

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
        curGunItemSlot = gunSlotParent.GetComponentInChildren<ItemSlotUI>();
    }

    public void UpdateGunInventorySlotUI(ItemData _curGunItem)
    {
        curGunItem = new InventoryItem(_curGunItem);
        curGunItemSlot.UpdateSlot(curGunItem);
    }

    public void UpdateCurrentBulletUI(int _maxBullet, int _loadedBullet)
    {
        magazineSize = _maxBullet;
        loadedBullet = _loadedBullet;

        //Text modify
    }

    //sawpinventory
}
