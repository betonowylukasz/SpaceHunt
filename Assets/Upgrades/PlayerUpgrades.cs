using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    public static PlayerUpgrades Instance;

    private float bonusDamageReduction = 0f;
    private float bonusStaminaCostReduction = 0f;
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
                bonusDamageReduction += upgrade.value;
                break;
            case Upgrade.UpgradeType.Stamina:
                PlayerController.Instance.UpgradePlayer(upgrade);
                bonusStaminaCostReduction = upgrade.value;
                break;
            case Upgrade.UpgradeType.StaminaRegeneration:
                PlayerController.Instance.UpgradePlayer(upgrade);
                bonusStaminaRegeneration = upgrade.value;
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
