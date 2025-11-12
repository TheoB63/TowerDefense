using UnityEngine;

[CreateAssetMenu(fileName = "TowerUpgrade", menuName = "Scriptable Objects/Upgrades/Tower Upgrade")]
public class TowerSpecificUpgrade : UpgradeBase
{
    public TowerType targetTowerType;
    public string upgradeMethodName; // Nom de la méthode à appeler
    public TowerSlot specificSlot;

    public override bool CanApply(TowerSlot slot, PlayerStats player)
    {
        if (player == null) return false;
        if (slot == null || !slot.HasTower()) return false;
        TowerSlot targetSlot = specificSlot ?? slot;

        ITower tower = slot.GetTower();
        return tower.GetTowerType() == targetTowerType;
    }

    public override void Apply(TowerSlot slot, PlayerStats player)
    {
        ITower tower = slot.GetTower();

        tower.ApplyUpgrade(upgradeMethodName);
        Debug.Log($"Upgrade {upgradeName} appliqué à {tower.GetTowerName()}");
    }

    public override string GetDisplayText(TowerSlot slot, PlayerStats player)
    {
        if (slot != null && slot.HasTower())
        {
            return $"{upgradeName} ({slot.GetTower().GetTowerName()})";
        }
        return upgradeName;
    }
}