using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_LaserPistol : Gun
{
    protected override void Shoot()
    {
        if (loadedBullet > 0 && shootTimer < 0.0)
        {
            Player.instance.animController.isBreath = true;
            if (cameraManager != null)
            {   // player 카메라 총 반동
                cameraManager.CameraShakeFromProfile(profile, impulseSource);
            }

            //Shoot Setting
            shootTimer = CalcShootDelay();
            loadedBullet--;
            continuousShootCnt++;
            SoundManager.instance.SetEffectSound(SoundType.Player, PlayerSfx.Breath);

            int bulletDamage = damage + Player.instance.reinforceAttack;
            float bulletScale = 1.0f;

            //장삥
            //10%로 데미지 1 증가
            SkillDB jpp110Data = SkillManager.instance.GetSkillDB(SeotdaHwatuCombination.JPP110);
            float jpp110Prob = SkillManager.instance.GetSkillProb(SeotdaHwatuCombination.JPP110);
            float randomVal = Random.Range(0.0f, 1.0f);
            if (SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.JPP110) && randomVal <= jpp110Prob)
            {
                bulletDamage += jpp110Data.damage;
                bulletScale = 1.5f;
            }

            //Create Bullet
            Vector2 dir = GetShootingDirection();
            float theta = Vector2.Angle(Vector2.right, dir);
            if (dir.y < 0)
                theta *= -1;
            GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, theta));
            PlayerLaserBullet bullet = bulletObj.GetComponent<PlayerLaserBullet>();

            bullet.BulletInitialize(bulletDamage, range, bulletSpeed, knockbackForce, dir, bulletScale);
            StartCoroutine(InactiveIsAttacking());

            //Gun Inventory Update
            GunManager.instance.UpdateCurrentGunBulletData(maxBullet, loadedBullet);
        }
    }
}
