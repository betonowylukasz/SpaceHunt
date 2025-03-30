using UnityEngine;

public class Shotgun : Weapon
{
    public int pelletCount = 5;
    public float spreadAngle = 15f;

    public override void Shoot()
    {
        if (Time.time >= nextFireTime)
        {
            for (int i = 0; i < pelletCount; i++)
            {
                Quaternion spreadRotation = Quaternion.Euler(0, 0, Random.Range(-spreadAngle, spreadAngle));
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * spreadRotation);
            }
            nextFireTime = Time.time + 1f / fireRate;
        }
    }
}
