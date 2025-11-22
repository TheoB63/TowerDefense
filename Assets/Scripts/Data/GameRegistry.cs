using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameRegistry : MonoBehaviour
{
    public static GameRegistry Instance;

    [Header("Registry")]
    public List<TowerData> towers;
    public List<UpgradeBase> upgrades;

    private void Awake()
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

    public TowerData GetTowerByType(TowerType _type)
    {
        return towers.Find(t => t.type == _type);
    }

    public UpgradeBase GetUpgradeByName(string _name)
    {
        UpgradeBase upgrade = null;
        if (upgrades.Any(u => u.name == _name))
        {
            upgrade = upgrades.Find(u => u.name == _name);
        }
        else
        {
            foreach (TowerData tower in towers)
            {
                if (tower.baseUpgrades.Any(u => u.name == _name))
                {
                    upgrade = tower.baseUpgrades.Find(u => u.name == _name);
                }
                else if (tower.lockedUpgrades.Any(u => u.upgrade.name == _name))
                {
                    upgrade = tower.lockedUpgrades.Find(u => u.upgrade.name == _name).upgrade;
                }
            }
        }
        return upgrade;
    }
}
