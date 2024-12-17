using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IGun
{
    void OnBulletAction(int maxBullet, int loadedBullet);
    void OnReloadAction(bool on);
}

public class GunController : MonoBehaviour, IGun
{
    public GunItemData baseGunData;
    public Transform gunParent;

    public Dictionary<GunItemData, GameObject> curHoldingGunItems;

    public Action<GunItemData> addGunAction;    // for data
    public Action<GunItemData> gunAction;       // for UI
    public Action<int, int> bulletAction;
    public Action<bool> reloadAction;

    [Header("Gun info")]
    GameObject currentGun;
    GunItemData curGunData;
    public GunItemData refCurGunData
    {
        get { return curGunData; }
        set
        {
            curGunData = value;
            gunAction(value);
        }
    }
    public int currentIdx;  // for add, swap index
    public float swapDelay = 0.1f;
    public float swapTimer;

    void Awake()
    {
        currentIdx = 0;
        curHoldingGunItems = new Dictionary<GunItemData, GameObject>();
        // 총 데이터 갱신 1 (gunData) & 2 (inventory는 presenter에)
        addGunAction += AddGunData;
    }

    void Start()
    {
        gunParent = Player.instance.transform.GetChild(1);
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Start") return;

        if (Player.instance.isCombatZone && !Player.instance.isInteraction)
        {
            if (curHoldingGunItems.Count == 1) return;

            swapTimer -= Time.deltaTime;
            float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
            if (scroll > 0 && swapTimer < 0.0f)
                SwapGun(true);
            if (scroll < 0 && swapTimer < 0.0f)
                SwapGun(false);
        }
    }

    public void AddGunData()
    {   // 최초 총 세팅
        addGunAction(baseGunData);
    }

    public void AddGunData(GunItemData itemData)
    {
        if (!curHoldingGunItems.TryGetValue(itemData, out GameObject obj))
        {
            GameObject _newGunObj = Instantiate(itemData.gunPrefab, gunParent.position, gunParent.rotation, gunParent);
            curHoldingGunItems.Add(itemData, _newGunObj);

            _newGunObj.GetComponent<Gun>().initItemData = itemData; // init될때마다 총알 자동 갱신
            currentIdx = curHoldingGunItems.Count - 1;

            InitActiveGun();    // 추가된 총으로 UI 갱신
        }
    }

    void SwapGun(bool up)
    {
        swapTimer = swapDelay;
        if (up)
        {
            currentIdx++;
            currentIdx %= curHoldingGunItems.Count;
        }
        else
        {
            currentIdx--;
            if (currentIdx < 0)
                currentIdx = curHoldingGunItems.Count - 1;
            currentIdx %= curHoldingGunItems.Count;
        }

        InitActiveGun();
    }

    public void DeleteGunData(GunItemData itemData)
    {
    }

    void InitActiveGun()
    {   // curGun 갱신하면서 프로퍼티 사용해서 이미지랑 총알 수 바인딩
        currentGun?.SetActive(false);
        
        currentGun = gunParent.GetChild(currentIdx).gameObject;
        refCurGunData = currentGun.GetComponent<Gun>().initItemData;

        currentGun.SetActive(true);
    }

    public void OnBulletAction(int maxBullet, int loadedBullet)
    {   // interface function
        bulletAction(maxBullet, loadedBullet);
    }

    public void OnReloadAction(bool on)
    {   // interface function
        reloadAction(on);
    }

    public Gun GetCurGunComponent()
    {
        return currentGun.GetComponent<Gun>();
    }
}
