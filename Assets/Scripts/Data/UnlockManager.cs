using System;
using System.Linq;
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

    private void Start()
    {
        foreach (var upgrade in GameRegistry.Instance.upgrades)
        {
            DataManager.Instance.unlockedGameUpgrades.Add(upgrade.name);
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
        if (GameRegistry.Instance.towers.Any(t => t.unlockLevel == level))
        {
            TowerType type = GameRegistry.Instance.towers.Find(t => t.unlockLevel == level).type;
            UnlockTower(type);
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