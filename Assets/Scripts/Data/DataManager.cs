using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    //Level Data
    public LevelConfig selectedLevel;
    public float gameSpeed = 1f;

    //Player

    //Levels
    //

    //Tower
    public List<TowerData> unlockedTowers = new List<TowerData>();
    public Dictionary<TowerData, TowerStats> towerStats = new Dictionary<TowerData, TowerStats>();

    //Upgrades
    public List<string> unlockedGameUpgrades = new List<string>();

    //Inv
    //

    //Events
    public static event Action<int> OnPlayerLevelUp;
    public static event Action<TowerData> OnTowerUnlock;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UnlockTower(TowerData _tower)
    {
        if (!unlockedTowers.Contains(_tower))
        {
            unlockedTowers.Add(_tower);
            OnTowerUnlock?.Invoke(_tower);
        }
    }

    public void UnlockGameUpgrade(UpgradeBase _upgrade)
    {
        if (!unlockedGameUpgrades.Contains(_upgrade.name))
        {
            unlockedGameUpgrades.Add(_upgrade.name);
        }
    }

    //Need to move in TowersMenu it's just an exemple
    public void UpgradeTower(TowerData tower, float damageBonus, float fireRateBonus)
    {
        if (!towerStats.ContainsKey(tower))
        {
            towerStats[tower] = new TowerStats();
        }

        towerStats[tower].damage += damageBonus;
    }

    public TowerStats GetTowerStats(TowerData tower)
    {
        if (!towerStats.ContainsKey(tower))
        {
            towerStats[tower] = new TowerStats();
        }
        return towerStats[tower];
    }
}
