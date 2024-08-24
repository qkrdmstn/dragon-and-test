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
            shootTimer = shootDelay;
            loadedBullet--;
            continuousShootCnt++;
            SoundManager.instance.SetEffectSound(SoundType.Player, PlayerSfx.Breath);

            int bulletDamage = damage;
            //장삥
            if (SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.JPP110))
            { //10%로 데미지 1 증가
                float prob = 0.1f;
                float randomVal = Random.Range(0.0f, 1.0f);
                if (randomVal <= prob)
                    bulletDamage++;
            }

            //Create Bullet
            Vector2 dir = GetShootingDirection();
            float theta = Vector2.Angle(Vector2.right, dir);
            if (dir.y < 0)
                theta *= -1;
            GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, theta));
            PlayerLaserBullet bullet = bulletObj.GetComponent<PlayerLaserBullet>();

            bullet.BulletInitialize(bulletDamage + Player.instance.reinforceAttack, range, bulletSpeed, knockbackForce, dir);
            StartCoroutine(InactiveIsAttacking());

            //Gun Inventory Update
            GunManager.instance.UpdateCurrentGunBulletData(maxBullet, loadedBullet);
        }
    }
}
