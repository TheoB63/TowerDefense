using System.Collections.Generic;
using UnityEngine;

public class LaserTower : MonoBehaviour, ITower
{
    [Header("Laser Settings")]
    public float baseDamage = 10f;
    public float baseRange = 5f;
    public float tickRate = 0.1f;
    public LineRenderer laserLine;

    [Header("Upgrade Levels")]
    public int damageLevel = 2;
    public int rangeLevel = 2;
    public bool hasMultiTarget = false;

    private float currentDamage;
    private float currentRange;
    private Transform currentTarget;

    private void Start()
    {
        UpdateStats();
        if (laserLine == null) laserLine = GetComponent<LineRenderer>();
    }

    private void UpdateStats()
    {
        currentDamage = baseDamage * Mathf.Pow(1.5f, damageLevel);
        currentRange = baseRange + (rangeLevel * 1f);
    }

    public TowerType GetTowerType() => TowerType.Laser;
    public string GetTowerName() => "Laser Tower";

    public void ApplyUpgrade(string upgradeName)
    {
        switch (upgradeName)
        {
            case "IncreaseDamage":
                damageLevel++;
                Debug.Log($"Laser: Damage level {damageLevel}");
                break;

            case "IncreaseRange":
                rangeLevel++;
                Debug.Log($"Laser: Range level {rangeLevel}");
                break;

            case "AddMultiTarget":
                hasMultiTarget = true;
                Debug.Log("Laser: Multi-target activé!");
                break;
        }
        UpdateStats();
    }

    public List<string> GetAvailableUpgrades()
    {
        var upgrades = new List<string>();

        if (damageLevel < 5) upgrades.Add("IncreaseDamage");
        if (rangeLevel < 3) upgrades.Add("IncreaseRange");
        if (!hasMultiTarget) upgrades.Add("AddMultiTarget");

        return upgrades;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, currentRange > 0 ? currentRange : baseRange);
    }
}
