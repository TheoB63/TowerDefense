using System.Collections.Generic;
using UnityEngine;

public class DroneSpawnerTower : MonoBehaviour, ITower
{
    [Header("Spawner Settings")]
    public GameObject dronePrefab;
    public int baseMaxDrones = 3;
    public float baseSpawnCooldown = 5f;

    [Header("Upgrade Levels")]
    public int maxDronesLevel = 0;
    public int spawnSpeedLevel = 0;
    public int droneHealthLevel = 0;

    private int currentMaxDrones;
    private float currentSpawnCooldown;
    private List<GameObject> activeDrones = new List<GameObject>();

    private void Start()
    {
        UpdateStats();
    }

    private void UpdateStats()
    {
        currentMaxDrones = baseMaxDrones + maxDronesLevel;
        currentSpawnCooldown = baseSpawnCooldown * Mathf.Pow(0.8f, spawnSpeedLevel);
    }

    public TowerType GetTowerType() => TowerType.Drone;
    public string GetTowerName() => "Drone Spawner";

    public void ApplyUpgrade(string upgradeName)
    {
        switch (upgradeName)
        {
            case "IncreaseMaxDrones":
                maxDronesLevel++;
                Debug.Log($"Spawner: Max drones = {currentMaxDrones}");
                break;

            case "ReduceSpawnTime":
                spawnSpeedLevel++;
                Debug.Log($"Spawner: Cooldown = {currentSpawnCooldown}s");
                break;

            case "ImproveDroneHealth":
                droneHealthLevel++;
                Debug.Log($"Spawner: Drone health level {droneHealthLevel}");
                break;
        }
        UpdateStats();
    }

    public List<string> GetAvailableUpgrades()
    {
        var upgrades = new List<string>();

        if (maxDronesLevel < 7) upgrades.Add("IncreaseMaxDrones");
        if (spawnSpeedLevel < 5) upgrades.Add("ReduceSpawnTime");
        if (droneHealthLevel < 5) upgrades.Add("ImproveDroneHealth");

        return upgrades;
    }
}