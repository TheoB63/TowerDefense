using System.Collections.Generic;
using UnityEngine;

public enum TowerType
{
    Basic,
    Railgun,
    Laser,
    Drone,
    SlowArea,
}

[CreateAssetMenu(fileName = "TowerData", menuName = "TowerDefense/Data/TowerData")]
public class TowerData : ScriptableObject
{
    [Header("TowerType")]
    public TowerType type;
    public string towerName;
    public GameObject prefab;
    public Sprite sprite;

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