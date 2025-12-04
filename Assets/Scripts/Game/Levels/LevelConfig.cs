using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

//EnemySpawnData
[System.Serializable]
public class EnemySpawnData
{
    public EnemyType type = EnemyType.Mob;
    public int index = 0;

    public float spawnTime;

    public GameObject GetPrefab(LevelConfig levelConfig)
    {
        switch (type)
        {
            case EnemyType.Mob:
                if (index <= levelConfig.mobPrefabs.Count && index >= 0)
                {
                    return levelConfig.mobPrefabs[index];
                }
                break;
            case EnemyType.boss:
                if (index <= levelConfig.bossPrefabs.Count && index >= 0)
                {
                    return levelConfig.bossPrefabs[index];
                }
                break;
            default:
                break;
        }
        return null;
    }

    public bool IsBoss()
    {
        return type == EnemyType.boss;
    }

    public string GetSlotName(LevelConfig levelConfig)
    {
        if (IsBoss())
        {
            return $"Boss{index + 1}";
        }
        else
        {
            return $"Mob{index + 1}";
        }
    }

    public float GetXpValue(LevelConfig levelConfig)
    {
        GameObject prefab = GetPrefab(levelConfig);
        if (prefab == null)
        {
            return 0;
        }

        Enemy enemyScript = prefab.GetComponent<Enemy>();
        if (enemyScript == null)
        {
            return 0;
        }

        return enemyScript.Xp;
    }

    public Color GetPreviewColor(LevelConfig levelConfig)
    {
        GameObject prefab = GetPrefab(levelConfig);
        if (prefab == null)
        {
            return Color.gray;
        }

        Enemy enemyScript = prefab.GetComponent<Enemy>();
        if (enemyScript == null)
        {
            return Color.white;
        }

        return enemyScript.previewColor;
    }

    public Sprite GetSprite(LevelConfig levelConfig)
    {
        GameObject prefab = GetPrefab(levelConfig);
        if (prefab == null)
        {
            return null;
        }

        SpriteRenderer spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            return null;
        }

        return spriteRenderer.sprite;
    }

    public string GetEnemyName(LevelConfig levelConfig)
    {
        GameObject prefab = GetPrefab(levelConfig);
        if (prefab != null)
        {
            return prefab.name;
        }
        return $"[{GetSlotName(levelConfig)} - Missing]";
    }

    public void CleanupIndex(LevelConfig levelConfig)
    {
        switch (type)
        {
            case EnemyType.Mob:
                if (index >= levelConfig.mobPrefabs.Count)
                {
                    index = levelConfig.mobPrefabs.Count > 0 ? 0 : -1;
                }
                break;
            case EnemyType.boss:
                if (index >= levelConfig.bossPrefabs.Count)
                {
                    index = levelConfig.mobPrefabs.Count > 0 ? 0 : -1;
                }
                break;
            default:
                break;
        }
    }
}

[System.Serializable]
public class StuffReward
{
    public int stuffLevel;
    public Rarity rarity;
    public float chance;
}

//LevelConfig
[CreateAssetMenu(fileName = "Level", menuName = "TowerDefense/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Level Info")]
    public int levelNumber = 1;
    public string levelName = "Level X";
    public int damageMultiplier = 1;
    public int hpMultiplier = 1;

    [Header("Xp")]
    public float baseXP = 0;

    [Header("Rewards")]
    public int xp = 15;
    public int gold = 150;
    public List<StuffReward> stuffRewards = new List<StuffReward>();
    public int nbStuffBlueprints;
    public int nbTowerFragments;


    [Header("Enemy Prefabs")]
    public List<GameObject> mobPrefabs = new List<GameObject>();
    public List<GameObject> bossPrefabs = new List<GameObject>();

    [Header("Enemies Timeline")]
    public List<EnemySpawnData> enemies = new List<EnemySpawnData>();

    

    public int GetTotalEnemies()
    {
        return enemies.Count;
    }

    public float GetLevelDuration()
    {
        if (enemies.Count <= 0)
        {
            return 0;
        }

        float maxTime = 0;
        foreach (var enemy in enemies)
        {
            if (enemy.spawnTime > maxTime)
            {
                maxTime = enemy.spawnTime;
            }
        }
        return maxTime;
    }


    public int CalculateLevel(float totalXP)
    {
        int level = 0;
        float baseXpNeeded = 2f;
        float xpNeeded = baseXpNeeded;
        float xpNeededMultiplier = 1.5f;

        while (totalXP > xpNeeded)
        {
            level++;
            totalXP -= xpNeeded;
            xpNeeded += baseXpNeeded * xpNeededMultiplier;
        }
        return level;
    }

    public float GetTotalXP()
    {
        float total = 0;
        foreach (var enemy in enemies)
        {
            total += enemy.GetXpValue(this);
        }
        return total;
    }

    public int GetStartLevel()
    {
        return CalculateLevel(baseXP);
    }

    public int GetEndLevel()
    {
        return CalculateLevel(baseXP + GetTotalXP());
    }

    public int GetBossCount()
    {
        return enemies.Count(e => e.IsBoss());
    }

    public int GetMobCount()
    {
        return enemies.Count(e => !e.IsBoss());
    }

    //In Editor
#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (var enemy in enemies)
        {
            enemy.CleanupIndex(this);
        }
    }

    [ContextMenu("Sort Enemies by Time")]
    public void SortEnemiesByTime()
    {
        if (enemies != null && enemies.Count > 1)
        {
            enemies = enemies.OrderBy(e => e.spawnTime).ToList();
            EditorUtility.SetDirty(this);
        }
    }

    [ContextMenu("Cleanup All Enemy Indices")]
    public void CleanupAllIndices()
    {
        foreach (var enemy in enemies)
        {
            enemy.CleanupIndex(this);
        }
        EditorUtility.SetDirty(this);
    }
#endif
}