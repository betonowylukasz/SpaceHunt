using System;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    public Weapon[] equipedWeapons;
    public Weapon[] availableWeapons;
    private int currentWeaponIndex = 0;
    private Weapon currentWeapon;
    public GameObject crosshair;
    public float weaponRadius = 0.08f;
    public float rotationSpeed = 20f;
    public Text ammoText;

    public Slider AmmoSlider;

    public event Action OnReloadAction;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        SaveData saveData = SaveManager.Instance.CurrentSaveData;
        equipedWeapons = new Weapon[2];

        equipedWeapons[0] = availableWeapons[saveData.selectedWeapons[0]];
        equipedWeapons[1] = availableWeapons[saveData.selectedWeapons[1]];
        currentWeaponIndex = saveData.selectedWeapon;

        LoadAmmo();

        EquipWeapon(currentWeaponIndex);
    }

    private void FixedUpdate()
    {
        RotateWeaponTowardsCrosshair();

        if (AmmoSlider != null && currentWeapon != null)
        {
            float reloadProgress = currentWeapon.GetReloadProgress();

            if (reloadProgress == 0f)
            {
                AmmoSlider.value = (float)currentWeapon.ammoInClip / currentWeapon.maxClip;
            }
            else
            {
                AmmoSlider.value = reloadProgress;
            }
        }
    }

    public void SaveAmmo()
    {
        equipedWeapons[0].SaveAmmo(0);
        equipedWeapons[1].SaveAmmo(1);
    }

    public void LoadAmmo()
    {
        equipedWeapons[0].LoadAmmo(0);
        equipedWeapons[1].LoadAmmo(1);
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
        currentWeapon = equipedWeapons[currentWeaponIndex];
        currentWeapon.Equip();
        UpdateAmmoText();

        SaveManager.Instance.CurrentSaveData.selectedWeapon = currentWeaponIndex;
    }

    public void Shoot()
    {
        if (currentWeapon.CanShoot())
        {
            currentWeapon.Shoot();
            currentWeapon.TakeAmmo();
            UpdateAmmoText();
        }
    }

    public void Reload()
    {
        if (currentWeapon.CanReload())
        {
            OnReloadAction?.Invoke();
            currentWeapon.Reload();
        }
        UpdateAmmoText();
    }

    public void UpdateAmmoText ()
    {
        ammoText.text = currentWeapon.ammoInClip.ToString() + "/" + currentWeapon.ammoReserve.ToString();
    }

    public Weapon[] GetWeapons()
    {
        return equipedWeapons;
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

    public void SelectWeapon(int equipedIndex, int availableIndex)
    {
        equipedWeapons[equipedIndex] = availableWeapons[availableIndex];
        equipedWeapons[equipedIndex].resetAmmo();
        EquipWeapon(equipedIndex);

        equipedWeapons[equipedIndex].SaveAmmo(equipedIndex);
        SaveManager.Instance.CurrentSaveData.selectedWeapons[equipedIndex] = availableIndex;
        SaveManager.Instance.Save();
    }

    public string EquipedWeaponName(int index)
    {
        return equipedWeapons[index].name;
    }

    public Sprite EquipedWeaponSprite(int index)
    {
        return equipedWeapons[index].icon;
    }
}
