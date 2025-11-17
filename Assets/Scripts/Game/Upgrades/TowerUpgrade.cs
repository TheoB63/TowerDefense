using UnityEngine;

[CreateAssetMenu(fileName = "TowerUpgrade", menuName = "TowerDefense/Upgrades/Tower Upgrade")]
public class TowerUpgrade : UpgradeBase
{
    public TowerType targetTowerType;
    public string upgradeMethodName; // Nom de la méthode à appeler
    public TowerSlot specificSlot;

    public override bool CanApply(TowerSlot _slot, PlayerStats _player)
    {
        if (_player == null) return false;
        if (_slot == null || !_slot.HasTower()) return false;
        TowerSlot targetSlot = specificSlot ?? _slot;

        ITower tower = _slot.GetTower();
        return tower.GetTowerType() == targetTowerType;
    }

    public override void Apply(TowerSlot _slot, PlayerStats _player)
    {
        ITower tower = _slot.GetTower();

        tower.ApplyUpgrade(upgradeMethodName);
    }

    public override string GetDisplayText(TowerSlot _slot, PlayerStats _player)
    {
        if (_slot != null && _slot.HasTower())
        {
            return $"{upgradeName} ({_slot.GetTower().GetTowerName()})";
        }
        return upgradeName;
    }
}