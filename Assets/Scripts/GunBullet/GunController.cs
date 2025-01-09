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
    public HashSet<GunItemData> curGunItems;

    public Action<GunItemData> addGunAction;    // for data & inventory
    public Action<GunItemData> gunAction;       // for cur Gun UI
    public Action<int, int> bulletAction;
    public Action<bool> reloadAction;

    [Header("Gun info")]
    public GameObject currentGun;
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
        curGunItems = new HashSet<GunItemData>();
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
            if (curGunItems.Count == 1) return;

            swapTimer -= Time.deltaTime;
            float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
            if (scroll > 0 && swapTimer < 0.0f)
                SwapGun(true);
            if (scroll < 0 && swapTimer < 0.0f)
                SwapGun(false);
        }
    }

    public bool CheckDuplicateGun(GunItemData item)
    {   // 중복여부 체크 후 총 추가 작업 수행
        if (!curGunItems.TryGetValue(item, out GunItemData realItem))
            return false;
        else return true;
    }

    public void AddGunData()
    {   // 최초 총 세팅
        if (!CheckDuplicateGun(baseGunData))
        {
            AddGunAction(baseGunData);
        }
    }

    public void AddGunAction(GunItemData item)
    {
        addGunAction(item);
    }   // 아래 함수 바인딩

    void AddGunData(GunItemData itemData)
    {
        currentGun = Instantiate(itemData.gunPrefab, gunParent.position, gunParent.rotation, gunParent);
        curGunItems.Add(itemData);

        currentGun.GetComponent<Gun>().initItemData = itemData; // init될때마다 총알 자동 갱신
        currentIdx = curGunItems.Count - 1;

        InitActiveGun();    // 추가된 총으로 UI 갱신
    }

    void SwapGun(bool up)
    {
        swapTimer = swapDelay;
        if (up)
        {
            currentIdx++;
            currentIdx %= curGunItems.Count;
        }
        else
        {
            currentIdx--;
            if (currentIdx < 0)
                currentIdx = curGunItems.Count - 1;
            currentIdx %= curGunItems.Count;
        }

        InitActiveGun();
    }

    public void ClearGunDatas()
    {   // 모든 총을 삭제하고 기본 총을 새롭게 생성
        refCurGunData = null;
        foreach(Transform curGun in gunParent.GetComponentsInChildren<Transform>(true))
        {
            if (curGun == gunParent) continue;
            else Destroy(curGun.gameObject);
        }

        curGunItems.Clear();
        AddGunData();
    }

    void InitActiveGun()
    {   // curGun 갱신하면서 프로퍼티 사용해서 이미지랑 총알 수 바인딩
        if(currentGun) currentGun.SetActive(false); // 순간적으로 바뀔때 보여서 잠깐 끔
        
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
