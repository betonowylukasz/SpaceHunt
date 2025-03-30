using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Weapon[] availableWeapons;
    private int currentWeaponIndex = 0;
    private Weapon currentWeapon;
    public GameObject crosshair;
    public float weaponRadius = 0.08f;
    public float rotationSpeed = 20f;

    void Start()
    {
        EquipWeapon(currentWeaponIndex);
    }

    private void FixedUpdate()
    {
        RotateWeaponTowardsCrosshair();
    }

    private void RotateWeaponTowardsCrosshair()
    {
        Vector3 characterPosition = transform.position;
        Vector3 crosshairPosition = crosshair.transform.position;

        Vector3 direction = crosshairPosition - characterPosition;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float radius = weaponRadius;
        float xOffset = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float yOffset = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        Vector3 weaponOffset = new Vector3(xOffset, yOffset, 0f);
        currentWeapon.transform.localPosition = weaponOffset;

        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        currentWeapon.transform.rotation = Quaternion.Slerp(currentWeapon.transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        SpriteRenderer weaponSpriteRenderer = currentWeapon.GetComponent<SpriteRenderer>();
        if (direction.x < 0)
        {
            if (weaponSpriteRenderer.flipY == false)
            {
                Vector3 firePointLocalPos = currentWeapon.firePoint.localPosition;
                firePointLocalPos.y = -firePointLocalPos.y;
                currentWeapon.firePoint.localPosition = firePointLocalPos;
            }
            weaponSpriteRenderer.flipY = true;
          

        }
        else
        {
            if (weaponSpriteRenderer.flipY == true)
            {
                Vector3 firePointLocalPos = currentWeapon.firePoint.localPosition;
                firePointLocalPos.y = -firePointLocalPos.y;
                currentWeapon.firePoint.localPosition = firePointLocalPos;
            }
            weaponSpriteRenderer.flipY = false;
        }
    }

    public void EquipWeapon(int index)
    {
        if (currentWeapon != null)
        {
            currentWeapon.Unequip();
        }

        currentWeaponIndex = index;
        currentWeapon = availableWeapons[currentWeaponIndex];
        currentWeapon.Equip();
    }

    public void Shoot()
    {
        currentWeapon.Shoot();
    }

    public void Weapon1()
    {
        currentWeaponIndex = 0;
        EquipWeapon(0);
    }

    public void Weapon2()
    {
        currentWeaponIndex = 0;
        EquipWeapon(1);
    }
}
