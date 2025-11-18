using System.Collections.Generic;
using UnityEngine;

public enum TowerType
{
    Projectile,
    Railgun,
    Laser,
    Drone,
    SlowArea,
}

[CreateAssetMenu(fileName = "TowerData", menuName = "TowerDefense/Data/TowerData")]
public class TowerData : ScriptableObject
{
    public TowerType Type;
    public string Name;
    public GameObject prefab;

    public float baseDamage;

    public List<UpgradeBase> possibleUpgrades;
}