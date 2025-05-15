using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Upgrades/Upgrade")]
public class Upgrade : ScriptableObject
{
    public string upgradeName;
    public string description;
    public Sprite icon;
    public UpgradeType type;
    public int value;

    public enum UpgradeType { Health, DamageReduction, Stamina, StaminaRegeneration, Ammo, Speed, Damage, FireRate }
}
