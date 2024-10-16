using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class GunInventoryData : MonoBehaviour
{
    public static GunInventoryData instance;

    public InventoryItem curGunItem;
    public int loadedBullet;
    public int maxBullet;

    [Header("GunInventory UI")]
    [SerializeField] private Transform gunSlotParent;
    [SerializeField] private TextMeshProUGUI bulletText;
    private GunInventorySlotUI curGunItemSlot;

    private void Awake()
    {
        if (instance == null)
        { //생성 전이면
            instance = this; //생성
            curGunItemSlot = gunSlotParent.GetComponentInChildren<GunInventorySlotUI>(true);    //비활성화된 자식객체도 할당할 수 있게 true
        }
        else if (instance != this)
        { //이미 생성되어 있으면
            Destroy(this.gameObject); //새로만든거 삭제
        }

        DontDestroyOnLoad(this.gameObject); //씬이 넘어가도 오브젝트 유지
    }

    public void UpdateGunInventorySlotUI(ItemData _curGunItem)
    {
        curGunItem = new InventoryItem(_curGunItem);
        curGunItemSlot.UpdateSlot(curGunItem);
    }

    public void UpdateCurrentBulletUI(int _maxBullet, int _loadedBullet)
    {
        maxBullet = _maxBullet;
        loadedBullet = _loadedBullet;
        if(maxBullet >= 10000) //총알 무한대
            bulletText.text = "<size=40>" + loadedBullet.ToString() + "</size><size=20> / ∞</size>";
        else
            bulletText.text = "<size=40>" + loadedBullet.ToString() + "</size><size=20> / " + maxBullet.ToString() + "</size>";
    }

    //sawpinventory
}
