using System.Collections.Generic;
using UnityEngine;

public class BasicTower : MonoBehaviour,ITower
{
    [Header("Tower Settings")]
    public GameObject projectilesPrefab;
    public float baseDamage = 10f;
    public float baseRange = 5f; 
    public float baseAttackSpeed = 1f;

    [Header("Upgrade Levels")]
    public int damageLevel = 2;
    public int rangeLevel = 2;
    public int attackSpeedLevel = 1;
    public bool hasMultiTarget = false;

    private float currentDamage;
    private float currentRange;
    private float currentAttackSpeed;
    private Transform currentTarget;

    private void Start()
    {
        UpdateStats();
    }

    private void UpdateStats()
    {
        currentDamage = baseDamage * Mathf.Pow(1.5f, damageLevel);
        currentRange = baseRange + (rangeLevel * 1f);
        currentAttackSpeed = baseAttackSpeed + (attackSpeedLevel * 1f);
    }

    void Update()
    {
        
    }

    public TowerType GetTowerType() => TowerType.Projectile;
    public string GetTowerName() => "Basic Tower";

    public void ApplyUpgrade(string upgradeName)
    {
        switch (upgradeName)
        {
            case "IncreaseDamage":
                damageLevel++;
                break;
            case "IncreaseRange":
                rangeLevel++;
                break;
            case "AddMultiTarget":
                // Logique pour cibler plusieurs ennemis
                break;
        }
        UpdateStats();
    }

    public List<string> GetAvailableUpgrades()
    {
        var upgrades = new List<string>();
        /*if (damageLevel < 5) */
        upgrades.Add("IncreaseDamage");
        /*if (rangeLevel < 3)*/ 
        upgrades.Add("IncreaseRange");
        return upgrades;
    }
}
