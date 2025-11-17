using UnityEngine;

[CreateAssetMenu(fileName = "PlayerUpgrade", menuName = "TowerDefense/Upgrades/Player Upgrade")]
public class PlayerUpgrade : UpgradeBase
{
    public string upgradeMethodName;

    public override bool CanApply(TowerSlot _slot, PlayerStats _player)
    {
        return _player != null;
    }

    public override void Apply(TowerSlot _slot, PlayerStats _player)
    {
        _player.ApplyUpgrade(upgradeMethodName);
    }

    public override string GetDisplayText(TowerSlot _slot, PlayerStats _player)
    {
        return $"{upgradeName} (Joueur)";
    }
}