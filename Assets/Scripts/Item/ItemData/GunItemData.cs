using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun Item Data", menuName = "Data/GunItem")]
public class GunItemData : ItemData
{
    public GunData gunData;

    [Header("Prefabs")]
    public GameObject gunPrefab;
    public GameObject bulletPrefab;
}
