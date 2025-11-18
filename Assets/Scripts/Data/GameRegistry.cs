using System.Collections.Generic;
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
        return towers.Find(t => t.Type == _type);
    }
}
