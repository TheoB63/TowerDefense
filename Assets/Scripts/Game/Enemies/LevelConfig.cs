using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class EnemySpawnData
{
    public EnemyType type = EnemyType.Mob;
    public int index = 0;


    [Tooltip("À quel moment spawn cet ennemi (en secondes depuis le début du level)")]
    public float spawnTime;

    // Récupère le prefab depuis le LevelConfig
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

    // Vérifie si c'est un boss
    public bool IsBoss()
    {
        return type == EnemyType.boss;
    }

    // Récupère le type d'ennemi sous forme de string pour affichage
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

    // Récupère l'XP
    public float GetXPValue(LevelConfig levelConfig)
    {
        GameObject prefab = GetPrefab(levelConfig);
        if (prefab == null) return 0;

        Enemy enemyScript = prefab.GetComponent<Enemy>();
        if (enemyScript == null) return 0;
        return enemyScript.Xp;
    }

    // Récupère la couleur pour le preview
    public Color GetPreviewColor(LevelConfig levelConfig)
    {
        GameObject prefab = GetPrefab(levelConfig);
        if (prefab == null) return Color.gray;

        Enemy enemyScript = prefab.GetComponent<Enemy>();
        if (enemyScript == null) return Color.white;
        return enemyScript.previewColor;
    }

    // Récupère le sprite pour l'affichage
    public Sprite GetSprite(LevelConfig levelConfig)
    {
        GameObject prefab = GetPrefab(levelConfig);
        if (prefab == null) return null;

        SpriteRenderer spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) return null;
        return spriteRenderer.sprite;
    }

    // Récupère le nom de l'ennemi
    public string GetEnemyName(LevelConfig levelConfig)
    {
        GameObject prefab = GetPrefab(levelConfig);
        return prefab != null ? prefab.name : $"[{GetSlotName(levelConfig)} - MISSING]";
    }

    // Nettoie les index invalides (appelé automatiquement)
    public void CleanupIndices(LevelConfig levelConfig)
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
                    index = -1;
                    index = levelConfig.mobPrefabs.Count > 0 ? 0 : -1;
                }
                break;
            default:
                break;
        }
    }
}

[CreateAssetMenu(fileName = "Level", menuName = "TowerDefense/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Level Info")]
    public int levelNumber = 1;
    public string levelName = "Level 1";
    public int damageMultiplier = 1;
    public int hpMultiplier = 1;

    [Header("Enemy Prefabs Assignment")]
    [Tooltip("Liste des prefabs de mobs normaux")]
    public List<GameObject> mobPrefabs = new List<GameObject>();

    [Tooltip("Liste des prefabs de boss")]
    public List<GameObject> bossPrefabs = new List<GameObject>();

    [Header("Enemies Timeline")]
    [Tooltip("Structure du level : définit QUAND spawn QUEL type d'ennemi")]
    public List<EnemySpawnData> enemies = new List<EnemySpawnData>();

    [Header("Player Level Calculation")]
    [Tooltip("XP de base que le joueur a au début de ce level")]
    public float baseXP = 0;

    // ====== MODIFIE CETTE FONCTION SELON TON SYSTÈME DE LEVEL ======
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
    // ================================================================

    // === Fonctions utiles pour l'éditeur et le gameplay ===

    public int GetTotalEnemies()
    {
        return enemies.Count;
    }

    public float GetTotalXP()
    {
        float total = 0;
        foreach (var enemy in enemies)
        {
            total += enemy.GetXPValue(this);
        }
        return total;
    }

    public float GetLevelDuration()
    {
        if (enemies.Count == 0) return 0;
        float maxTime = 0;
        foreach (var enemy in enemies)
        {
            if (enemy.spawnTime > maxTime)
                maxTime = enemy.spawnTime;
        }
        return maxTime;
    }

    public int GetStartLevel()
    {
        return CalculateLevel(baseXP);
    }

    public int GetEndLevel()
    {
        return CalculateLevel(baseXP + GetTotalXP());
    }

    // Compter les boss dans le level
    public int GetBossCount()
    {
        return enemies.Count(e => e.IsBoss());
    }

    // Compter les mobs dans le level
    public int GetMobCount()
    {
        return enemies.Count(e => !e.IsBoss());
    }

    // ====== NETTOYAGE AUTOMATIQUE DES INDEX INVALIDES ======
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Nettoyer tous les index invalides quand on modifie les listes
        foreach (var enemy in enemies)
        {
            enemy.CleanupIndices(this);
        }
    }

    [ContextMenu("Sort Enemies by Time")]
    public void SortEnemiesByTime()
    {
        if (enemies != null && enemies.Count > 1)
        {
            enemies = enemies.OrderBy(e => e.spawnTime).ToList();
            EditorUtility.SetDirty(this);
            Debug.Log($"✅ Sorted {enemies.Count} enemies by spawn time");
        }
    }

    [ContextMenu("Duplicate Level Structure")]
    public void DuplicateLevelStructure()
    {
        Debug.Log("💡 To duplicate: Right-click this asset → Duplicate, then change the prefab assignments!");
    }

    [ContextMenu("Cleanup All Enemy Indices")]
    public void CleanupAllIndices()
    {
        foreach (var enemy in enemies)
        {
            enemy.CleanupIndices(this);
        }
        EditorUtility.SetDirty(this);
        Debug.Log("✅ Cleaned up all enemy indices");
    }
#endif
}