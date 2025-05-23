using UnityEngine;

public class Blaster : Weapon
{
    public AudioClip shootClip;

    private AudioSource audioSource;
    
    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public override void Shoot()
    {
        if (Time.time >= nextFireTime)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            nextFireTime = Time.time + 1f / fireRate;
            SoundController.Instance.PlaySound(shootClip, 0.25f, 1f);
        }
    }
}
