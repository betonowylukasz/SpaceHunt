using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float fireRate;
    public Transform firePoint;
    public GameObject bulletPrefab;

    protected float nextFireTime = 0f;

    public abstract void Shoot();

    public virtual void Equip()
    {
        gameObject.SetActive(true);
    }

    public virtual void Unequip()
    {
        gameObject.SetActive(false);
    }
}

