using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    //Player
    [Header("Managers")]
    public PlayerManager playerManager;
    public UnlockManager unlockManager;

    //Level Data
    [Header("Level")]
    public LevelConfig selectedLevel;
    public float gameSpeed = 1f;



    //Levels
    //

    [Header("Towers")]
    public List<TowerStats> unlockedTowers = new List<TowerStats>();

    [Header("Upgrades")]
    public List<string> unlockedGameUpgrades = new List<string>();

    //Inv
    //

    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadGame()
    {
        bool save = false; //need to replace with verification
        if (save)//save
        {
            LoadSave();
        }
        else
        {
            CreateSave();
        }
    }

    void CreateSave()
    {
        playerManager = this.AddComponent<PlayerManager>();
        unlockManager = this.AddComponent<UnlockManager>();
    }

    void LoadSave()
    {

    }

    //public void UnlockTower(TowerData _tower)
    //{
    //    if (!unlockedTowers.Contains(_tower.name))
    //    {
    //        unlockedTowers.Add(_tower);
    //        OnTowerUnlock?.Invoke(_tower);
    //    }
    //}

    //public void UnlockGameUpgrade(UpgradeBase _upgrade)
    //{
    //    if (!unlockedGameUpgrades.Contains(_upgrade.name))
    //    {
    //        unlockedGameUpgrades.Add(_upgrade.name);
    //    }
    //}

    //Need to move in TowersMenu it's just an exemple
    //public void UpgradeTower(TowerData tower, float damageBonus, float fireRateBonus)
    //{
    //    if (!towerStats.ContainsKey(tower))
    //    {
    //        towerStats[tower] = new TowerStats();
    //    }

    //    towerStats[tower].damage += damageBonus;
    //}

    //public TowerStats GetTowerStats(TowerData tower)
    //{
    //    if (!towerStats.ContainsKey(tower))
    //    {
    //        towerStats[tower] = new TowerStats();
    //    }
    //    return towerStats[tower];
    //}
}
