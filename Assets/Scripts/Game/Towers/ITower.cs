using System.Collections.Generic;

public interface ITower
{
    TowerType GetTowerType();
    string GetTowerName();
    void ApplyUpgrade(string _upgradeName);
    List<UpgradeBase> GetAvailableUpgrades();
}