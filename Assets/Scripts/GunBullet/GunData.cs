using UnityEngine;

[CreateAssetMenu(fileName = "New Gun Data", menuName = "Data/Gun")]

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
    public float recoilIncrease;
    public float bulletSpeed;
    public float range;
    public float knockbackForce;
    public bool isAutomatic;

    public GunData(GunData _data)
    {
        this.damage = _data.damage;
        this.shootDelay = _data.shootDelay;
        this.reloadTime = _data.reloadTime;
        this.maxBullet = _data.maxBullet;
        this.magazineSize = _data.magazineSize;
        this.loadedBullet = _data.loadedBullet;
        this.maxRecoilDegree = _data.maxRecoilDegree;
        this.recoilIncrease = _data.recoilIncrease;
        this.bulletSpeed = _data.bulletSpeed;
        this.range = _data.range;
        this.knockbackForce = _data.knockbackForce;
        this.isAutomatic = _data.isAutomatic;
    }

    public void gunDataUpdate(Gun _gun)
    {
        this.damage = _gun.damage;
        this.shootDelay = _gun.shootDelay;
        this.reloadTime = _gun.reloadTime;
        this.maxBullet = _gun.refMaxBullet;
        this.magazineSize = _gun.magazineSize;
        this.loadedBullet = _gun.refLoadedBullet;
        this.maxRecoilDegree = _gun.maxRecoilDegree;
        this.recoilIncrease = _gun.recoilIncrease;
        this.bulletSpeed = _gun.bulletSpeed;
        this.range = _gun.range;
        this.knockbackForce = _gun.knockbackForce;
        this.isAutomatic = _gun.isAutomatic;
    }
}
