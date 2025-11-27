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

public enum DamageType
{
    Physic,
    Fire,
    Energy,
    Electric,
    Poison,
}

public enum StatType
{
    Damage,
    Cooldown,
    Range,
    ExplosionDamage,
    DamageRange,
    Duration,
    DamageInterval,
    NbAttack,
    Penetration
}

[System.Serializable]
public class TowerStatUpgrade
{
    public int level;
    public StatType type;
    public float value;
}

[System.Serializable]
public class UpgradeBaseLock
{
    public int level;
    public UpgradeBase upgrade;
}

[CreateAssetMenu(fileName = "TowerData", menuName = "TowerDefense/Data/TowerData")]
public class TowerData : ScriptableObject
{
    [Header("TowerType")]
    public TowerType type;
    public string towerName;
    public GameObject prefab;
    public Sprite sprite;
    public int unlockLevel;
    public int goldCost;
    public int goldCostIncrease;
    public int fragmentsCost;
    public int fragmentsCostIncrease;

    [Header("Common Stats")]
    public DamageType damageType = DamageType.Physic;
    public float baseDamage;
    public float baseCooldown;
    public float baseRange;

    [Header("Special Stats")]
    public float baseExplosionDamage;
    public float baseDamageRange;
    public float baseDuration;
    public float baseDamageInterval;
    public int baseNbAttack;
    public int basePenetration;

    [Header("Game Upgrades")]
    public List<UpgradeBase> baseUpgrades;
    public List<UpgradeBaseLock> lockedUpgrades;

    [Header("Stats Upgrades")]
    public float damageUpgrades = 1.1f;
    public List<TowerStatUpgrade> statsUpgrades;

    public float GetUpgradeStatForLevel(StatType _type, int _level)
    {
        float finalValue = 0;

        foreach (TowerStatUpgrade levelUpgrade in statsUpgrades)
        {
            if (levelUpgrade.level == _level)
            {
                if (levelUpgrade.type == _type)
                {
                    finalValue = levelUpgrade.value;
                }
            }
        }

        return finalValue;
    }

    public List<UpgradeBaseLock> GetUnlockedUpgradeForLevel(int _level)
    {
        List<UpgradeBaseLock> upgrades = new List<UpgradeBaseLock>();

        foreach (UpgradeBaseLock upgrade in lockedUpgrades)
        {
            if (upgrade.level == _level)
            {
                upgrades.Add(upgrade);
            }
        }

        return upgrades;
    }
}