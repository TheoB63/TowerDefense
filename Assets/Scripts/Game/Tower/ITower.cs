using System.Collections.Generic;

public interface ITower
{
    TowerType GetTowerType();
    string GetTowerName();
    void ApplyUpgrade(string _upgradeName);
    List<string> GetAvailableUpgrades();
}

public enum TowerType
{
    Laser,
    Projectile,
    Drone,
    SlowArea,
    Railgun,
    // etc.
}