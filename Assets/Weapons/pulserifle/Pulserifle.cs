using UnityEngine;

public class Pulserifle : Weapon
{
    public AudioClip shootClip;
    public float spreadAngle = 10f;

    private AudioSource audioSource;
    
    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public override void Shoot()
    {
        if (Time.time >= nextFireTime)
        {
            Quaternion spreadRotation = Quaternion.Euler(0, 0, Random.Range(-spreadAngle, spreadAngle));
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * spreadRotation);
            nextFireTime = Time.time + 1f / fireRate;
            SoundController.Instance.PlaySound(shootClip, 0.25f, 1f);
        }
    }
}
