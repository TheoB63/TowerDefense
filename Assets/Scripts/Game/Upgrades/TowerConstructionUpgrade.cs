using UnityEngine;

[CreateAssetMenu(fileName = "BuildTower", menuName = "TowerDefense/Upgrades/Build Tower")]
public class TowerConstructionUpgrade : UpgradeBase
{
    public TowerType towerType;
    public TowerSlot specificSlot;

    public override bool CanApply(TowerSlot _slot, PlayerStats _player)
    {
        if (_player == null) return false;
        TowerSlot targetSlot = specificSlot ?? _slot;
        return _slot != null && !_slot.HasTower();
    }

    public override void Apply(TowerSlot _slot, PlayerStats _player)
    {
        _slot.BuildTower(towerType);
    }

    public override string GetDisplayText(TowerSlot _slot, PlayerStats _player)
    {
        return $"Construire {upgradeName}";
    }
}
