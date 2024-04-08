using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class GunManager : MonoBehaviour
{
    public static GunManager instance;

    [Header("Gun Data")]
    public List<GunData> gunDataList;
    public Dictionary<GunData, GameObject> gunDictionary;
    public int gunNum;
    public Transform gunParent;

    [Header("Swap info")]
    public GameObject currentGun;
    public int currentIdx;
    public int currentGunLoadedBullet;
    public int currentGunMaxBullet;
    public float swapDelay = 0.1f;
    public float swapTimer;

    public GameObject tempPrefab;
    #region Components
    private Player player;
    #endregion

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

        gunDataList = new List<GunData>();
        gunDictionary = new Dictionary<GunData, GameObject>();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "Start")
            Initialize();
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name != "Start")
        {
            if (player.isCombatZone)
            {
                swapTimer -= Time.deltaTime;
                float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
                if (scroll > 0 && swapTimer < 0.0f)
                    SwapGun(true);
                if (scroll < 0 && swapTimer < 0.0f)
                    SwapGun(false);
            }

            if (Input.GetKeyDown(KeyCode.O))
                AddGun(tempPrefab);
        }
    }

    public void Initialize()
    {
        player = GameObject.FindObjectOfType<Player>();
        gunParent = player.gunParent.transform;

        LoadGun();
        InitActiveGun();
    }

    public void LoadGun()
    {
        int childCnt = gunParent.childCount;
        for (int i = 0; i < childCnt; i++)
        {
            GameObject gunObj = gunParent.GetChild(i).gameObject;
            GunData _data;
            if (i >= gunDataList.Count)
            {
                _data = new GunData(gunObj.GetComponent<Gun>().initData);
                AddGunDataList(_data);
            }
            else
                _data = gunDataList[i];

            LoadGunData(gunObj, _data);
        }

        for (int i = childCnt; i < gunDataList.Count; i++)
        {
            GunData _oldData = gunDataList[i];
            GameObject _newGunObj = Instantiate(gunDataList[i].gunPrefab, gunParent.position, gunParent.rotation, gunParent);

            LoadGunData(_newGunObj, _oldData);
        }
        gunNum = gunDataList.Count;
    }

    public void LoadGunData(GameObject _gun, GunData _oldData)
    {
        _gun.GetComponent<Gun>().InitGunData(_oldData);
        gunDictionary.Add(_oldData, _gun);
    }

    private void InitActiveGun() //Initialize Default Gun
    {
        for (int i = 0; i < gunParent.childCount; i++)
        {
            gunParent.GetChild(i).gameObject.SetActive(false);
        }

        if (gunParent.childCount == 1)
            currentIdx = 0;

        currentGun = gunParent.GetChild(currentIdx).gameObject;
        currentGun.SetActive(true);

        //Gun Inventory Update
        GunInventoryData.instance.UpdateGunInventorySlotUI(gunDataList[currentIdx].gunItemData);
        Gun _gun = gunParent.GetChild(currentIdx).GetComponent<Gun>();
        UpdateCurrentGunBulletData(_gun.maxBullet, _gun.loadedBullet);
    }

    public void SaveGunData() 
    {
        UpdateGunData();
        gunDictionary.Clear();
        Debug.Log("end");
    }

    private void UpdateGunData()
    {
        for (int i = 0; i < gunDataList.Count; i++)
        {
            if(gunDictionary.ContainsKey(gunDataList[i]))
            {
                GameObject _gunObj = gunDictionary[gunDataList[i]];
                Gun _gun = _gunObj.GetComponent<Gun>();
                gunDataList[i].gunDataUpdate(_gun);
                Debug.Log("asd");
            }
        }
    }

    void SwapGun(bool up)
    {
        swapTimer = swapDelay;
        if (up)
        {
            currentIdx++;
            currentIdx %= gunNum;
        }
        else
        {
            currentIdx--;
            if (currentIdx < 0)
                currentIdx = gunNum - 1;
            currentIdx %= gunNum;
        }
        InitActiveGun();
    }

    public void AddGun(GameObject _prefab)
    {
        //Add Gun & Initialize
        GameObject _newGunObj = Instantiate(_prefab, gunParent.position, gunParent.rotation, gunParent);
        Gun _newGun = _newGunObj.GetComponent<Gun>();
        _newGun.InitGunData(_newGun.initData);

        //Data Save
        GunData _data = new GunData(_newGun.initData);
        AddGunDataList(_data);
        gunDictionary.Add(_data, _newGunObj);
        gunNum++;

        //Current Use Gun Change
        currentIdx = gunNum - 1;
        InitActiveGun();
    }

    public void RemoveGun(GameObject gun)
    {
      
    }

    public void AddGunDataList(GunData _data)
    {
        gunDataList.Add(_data);

        //Inventory Update
        InventoryData.instance.AddGunItem(_data.gunItemData);
    }

    public void UpdateCurrentGunBulletData(int maxBullet, int loadedBullet)
    {
        currentGunMaxBullet = maxBullet;
        currentGunLoadedBullet = loadedBullet;

        GunInventoryData.instance.UpdateCurrentBulletUI(currentGunMaxBullet, currentGunLoadedBullet);
    }
}
