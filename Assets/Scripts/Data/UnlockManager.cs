using System;
using System.Linq;
using UnityEngine;

public class UnlockManager : MonoBehaviour
{
    public static UnlockManager Instance;

    //player
    PlayerManager m_playerManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            m_playerManager = GetComponent<PlayerManager>();
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

    void HandleLevelUp(int _level)
    {
        foreach (TowerData tower in GameRegistry.Instance.towers)
        {
            if (tower.unlockLevel == _level)
            {
                UnlockTower(tower.type);
            }
        }
    }

    void OnDisable()
    {
        m_playerManager.OnPlayerLevelUp -= HandleLevelUp;
    }

    void UnlockTower(TowerType _type)
    {
        //Create Tower Stats
        TowerStats stats = new TowerStats();
        stats.type = _type;
        stats.Create();
        stats.type = _type;
        DataManager.Instance.unlockedTowers.Add(stats);

        //Add Upgrade
        TowerData data = GameRegistry.Instance.GetTowerByType(_type);
        foreach (UpgradeBase upgrade in data.baseUpgrades)
        {
            DataManager.Instance.unlockedGameUpgrades.Add(upgrade.name);
        }
    }
}