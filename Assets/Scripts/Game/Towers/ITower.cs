using System.Collections.Generic;

public interface ITower
{
    TowerType GetTowerType();
    string GetTowerName();
    void ApplyUpgrade(string _upgradeName);
    List<UpgradeBase> GetAvailableUpgrades();
}

public enum TowerType
{
    Projectile,
    Railgun,
    Laser,
    Drone,
    SlowArea,
    // etc.
}