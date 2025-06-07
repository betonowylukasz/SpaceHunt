using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    public static PlayerUpgrades Instance;

    private float damageReceived = 100f;
    private float staminaCost = 100f;
    private float bonusStaminaRegeneration = 0f;
    private float bonusSpeed = 0f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void ApplyUpgrade(Upgrade upgrade)
    {
        switch (upgrade.type)
        {
            case Upgrade.UpgradeType.Health:
                PlayerController.Instance.UpgradePlayer(upgrade);
                break;
            case Upgrade.UpgradeType.DamageReduction:
                PlayerController.Instance.UpgradePlayer(upgrade);
                damageReceived += damageReceived * ((100 - upgrade.value) / 100f);
                break;
            case Upgrade.UpgradeType.Stamina:
                PlayerController.Instance.UpgradePlayer(upgrade);
                staminaCost += staminaCost * ((100 - upgrade.value) / 100f);
                break;
            case Upgrade.UpgradeType.StaminaRegeneration:
                PlayerController.Instance.UpgradePlayer(upgrade);
                bonusStaminaRegeneration += upgrade.value;
                break;
            case Upgrade.UpgradeType.Ammo:
                PlayerController.Instance.UpgradePlayer(upgrade);
                break;
            case Upgrade.UpgradeType.Speed:
                PlayerController.Instance.UpgradePlayer(upgrade);
                bonusSpeed += upgrade.value;
                break;
        }

        Debug.Log($"Upgrade applied: {upgrade.upgradeName}");
    }
}
