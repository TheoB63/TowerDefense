using UnityEngine;

[CreateAssetMenu(fileName = "PlayerUpgrade", menuName = "Scriptable Objects/Upgrades/Player Upgrade")]
public class PlayerUpgrade : UpgradeBase
{
    public string upgradeMethodName;

    public override bool CanApply(TowerSlot slot, PlayerStats player)
    {
        return player != null;
    }

    public override void Apply(TowerSlot slot, PlayerStats player)
    {
        player.ApplyUpgrade(upgradeMethodName);
        Debug.Log($"Upgrade joueur {upgradeName} appliqué");
    }

    public override string GetDisplayText(TowerSlot slot, PlayerStats player)
    {
        return $"{upgradeName} (Joueur)";
    }
}