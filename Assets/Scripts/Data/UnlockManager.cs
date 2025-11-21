using System;
using UnityEngine;

public class UnlockManager : MonoBehaviour
{
    public static UnlockManager Instance;

    //player
    PlayerManager playerManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            playerManager = GetComponent<PlayerManager>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {

    }
    void OnEnable()
    {
        GetComponent<PlayerManager>().OnPlayerLevelUp += HandleLevelUp;
    }

    void HandleLevelUp(int level)
    {
        if (playerManager.level == 1) //start new save
        {
            UnlockTower(TowerType.Basic);
        }
        else if (playerManager.level == 2)
        {
            UnlockTower(TowerType.Railgun);
            UnlockTower(TowerType.Laser);
        }
    }

    void OnDisable()
    {
        playerManager.OnPlayerLevelUp -= HandleLevelUp;
    }

    void UnlockTower(TowerType _type)
    {
        //Create Tower Stats
        TowerStats stats = new TowerStats();
        stats.type = _type;
        stats.Create();
        stats.type = _type;
        DataManager.Instance.unlockedTowers.Add(stats);
        Debug.Log("Add tower " + _type);

        //Add Upgrade
        TowerData data = GameRegistry.Instance.GetTowerByType(_type);
        foreach (UpgradeBase upgrade in data.baseUpgrades)
        {
            DataManager.Instance.unlockedGameUpgrades.Add(upgrade.name);
        }
    }
}