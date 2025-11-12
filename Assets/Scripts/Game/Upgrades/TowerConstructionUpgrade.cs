using UnityEngine;

[CreateAssetMenu(fileName = "BuildTower", menuName = "Scriptable Objects/Upgrades/Build Tower")]
public class TowerConstructionUpgrade : UpgradeBase
{
    public GameObject towerPrefab;
    public TowerType towerType;
    //public TowerSlot specificSlot;

    public override bool CanApply(TowerSlot slot, PlayerStats player)
    {
        if (player == null) return false;
        //if (specificSlot != null && slot != specificSlot) return false;
        return slot != null && !slot.HasTower();
    }

    public override void Apply(TowerSlot slot, PlayerStats player)
    {
        slot.BuildTower(towerPrefab);
        Debug.Log($"Tourelle {upgradeName} construite sur le slot {slot.name}");
    }

    public override string GetDisplayText(TowerSlot slot, PlayerStats player)
    {
        return $"Construire {upgradeName}";
    }
}
