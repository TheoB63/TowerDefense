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
    [Header("TowerType")]
    public TowerType Type;
    public string Name;
    public GameObject prefab;

    [Header("Common Stats")]
    public float baseDamage;
    public float baseCooldown;
    public float baseRange;

    [Header("Common Stats")]
    public float baseExplosionDamage;
    public float baseDamageRange;
    public float baseDuration;
    public float baseDamageInterval;
    public float baseNbAttack;
    public float basePenetration;

    public List<UpgradeBase> possibleUpgrades;
}