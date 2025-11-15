using System.Collections.Generic;
using UnityEngine;

public class DroneTower : Tower
{
    //[Header("Spawner Settings")]
    //public GameObject dronePrefab;
    //public int baseMaxDrones = 3;
    //public float baseSpawnCooldown = 5f;

    //[Header("Upgrade Levels")]
    //public int maxDronesLevel = 0;
    //public int spawnSpeedLevel = 0;
    //public int droneHealthLevel = 0;

    //private int currentMaxDrones;
    //private float currentSpawnCooldown;
    //private List<GameObject> activeDrones = new List<GameObject>();

    //private void Start()
    //{
    //    UpdateStats();
    //}

    //private void UpdateStats()
    //{
    //    currentMaxDrones = baseMaxDrones + maxDronesLevel;
    //    currentSpawnCooldown = baseSpawnCooldown * Mathf.Pow(0.8f, spawnSpeedLevel);
    //}

    //public TowerType GetTowerType() => TowerType.Drone;
    //public string GetTowerName() => "Drone Spawner";

    //public void ApplyUpgrade(string upgradeName)
    //{
    //    switch (upgradeName)
    //    {
    //        case "IncreaseMaxDrones":
    //            maxDronesLevel++;
    //            break;

    //        case "ReduceSpawnTime":
    //            spawnSpeedLevel++;
    //            break;

    //        case "ImproveDroneHealth":
    //            droneHealthLevel++;
    //            break;
    //    }
    //    UpdateStats();
    //}

    //public List<string> GetAvailableUpgrades()
    //{
    //    var upgrades = new List<string>();

    //    if (maxDronesLevel < 7) upgrades.Add("IncreaseMaxDrones");
    //    if (spawnSpeedLevel < 5) upgrades.Add("ReduceSpawnTime");
    //    if (droneHealthLevel < 5) upgrades.Add("ImproveDroneHealth");

    //    return upgrades;
    //}
}