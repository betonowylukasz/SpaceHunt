using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float fireRate;
    public float reloadDuration = 3f;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public int maxReserve;
    public int ammoReserve;
    public int ammoInClip;
    public int maxClip;

    public string name;
    public Sprite icon;

    protected float nextFireTime = 0f;
    protected float reloadTime = 0f;

    public abstract void Shoot();

    public virtual void Equip()
    {
        gameObject.SetActive(true);
    }

    public virtual void Unequip()
    {
        gameObject.SetActive(false);
    }

    public void TakeAmmo(int amount = 1)
    {
        ammoInClip -= amount;
        if (ammoInClip <= 0)
        {
            ammoInClip =  Math.Min(maxClip, ammoReserve);
            ammoReserve -= ammoInClip;
            nextFireTime = Time.time + reloadDuration;
            reloadTime = Time.time + reloadDuration;
        }
    }

    public bool CanShoot()
    {
        return ammoInClip > 0 && Time.time >= nextFireTime;
    }

    public void AddAmmo(int magazines)
    {
        ammoReserve += maxClip * magazines;

        if (ammoReserve > maxReserve)
        {
            ammoReserve = maxReserve;
        }
    }

    public void resetAmmo()
    {
        ammoReserve = maxReserve;
        ammoInClip = maxClip;
    }

    public float GetReloadProgress()
    {
        if (Time.time < reloadTime)
        {
            return 1 - (reloadTime - Time.time) / reloadDuration;
        }

        return 0f;
    }
}

