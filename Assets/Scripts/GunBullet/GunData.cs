using UnityEngine;

public class GunData : ScriptableObject
{
    [Header("Gun info")]
    public int damage = 1;
    public float shootDelay;
    public float reloadTime;
    public int maxBullet;
    public int magazineSize;
    public int loadedBullet;
    public float maxRecoilDegree;

    [Header("Item Data")]
    public ItemData gunItemData;

    [Header("Prefabs")]
    public GameObject gunPrefab;
    public GameObject bulletPrefab;

    public GunData(GunData _data)
    {
        this.damage = _data.damage;
        this.shootDelay = _data.shootDelay;
        this.reloadTime = _data.reloadTime;
        this.maxBullet = _data.maxBullet;
        this.magazineSize = _data.magazineSize;
        this.loadedBullet = _data.loadedBullet;
        this.maxRecoilDegree = _data.maxRecoilDegree;

        this.gunItemData = _data.gunItemData;

        this.gunPrefab = _data.gunPrefab;
        this.bulletPrefab = _data.bulletPrefab;
    }

    public void gunDataUpdate(Gun _gun)
    {
        this.damage = _gun.damage;
        this.shootDelay = _gun.shootDelay;
        this.reloadTime = _gun.reloadTime;
        this.maxBullet = _gun.maxBullet;
        this.magazineSize = _gun.magazineSize;
        this.loadedBullet = _gun.loadedBullet;
        this.maxRecoilDegree = _gun.maxRecoilDegree;

        this.gunPrefab = _gun.gunPrefab;
        this.bulletPrefab = _gun.bulletPrefab;
    }
}
