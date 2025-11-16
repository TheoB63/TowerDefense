using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Enum pour identifier le type d'ennemi
public enum EnemySlotType
{
    Mob1,
    Mob2,
    Mob3,
    Mob4,
    Mob5,
    Boss1,
    Boss2,
    Boss3,
    Boss4,
    Boss5
}

[System.Serializable]
public class EnemySpawnData
{
    [Tooltip("Type d'ennemi à spawn (Mob1, Mob2, Boss1...)")]
    public EnemySlotType slotType;

    [Tooltip("À quel moment spawn cet ennemi (en secondes depuis le début du level)")]
    public float spawnTime;

    // Récupère le prefab depuis le LevelConfig
    public GameObject GetPrefab(LevelConfig levelConfig)
    {
        return levelConfig.GetPrefabForSlot(slotType);
    }

    // Vérifie si c'est un boss
    public bool IsBoss()
    {
        return slotType == EnemySlotType.Boss1 ||
               slotType == EnemySlotType.Boss2 ||
               slotType == EnemySlotType.Boss3 ||
               slotType == EnemySlotType.Boss4 ||
               slotType == EnemySlotType.Boss5;
    }

    // Récupère l'XP (en passant le levelConfig pour avoir le prefab)
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
        return prefab != null ? prefab.name : $"[{slotType}]";
    }
}

[CreateAssetMenu(fileName = "Level", menuName = "TowerDefense/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Level Info")]
    public int levelNumber = 1;
    public string levelName = "Level 1";

    [Header("Enemy Prefabs Assignment")]
    [Tooltip("Liste des prefabs de mobs normaux (Mob1, Mob2, Mob3...)")]
    public List<GameObject> mobPrefabs = new List<GameObject>();

    [Tooltip("Liste des prefabs de boss (Boss1, Boss2, Boss3...)")]
    public List<GameObject> bossPrefabs = new List<GameObject>();

    [Header("Enemies Timeline")]
    [Tooltip("Structure du level : définit QUAND spawn QUEL type d'ennemi")]
    public List<EnemySpawnData> enemies = new List<EnemySpawnData>();

    [Header("Player Level Calculation")]
    [Tooltip("XP de base que le joueur a au début de ce level")]
    public float baseXP = 0;

    // ====== RÉCUPÉRER LE PREFAB SELON LE SLOT ======
    public GameObject GetPrefabForSlot(EnemySlotType slotType)
    {
        switch (slotType)
        {
            // Mobs
            case EnemySlotType.Mob1:
                return mobPrefabs.Count > 0 ? mobPrefabs[0] : null;
            case EnemySlotType.Mob2:
                return mobPrefabs.Count > 1 ? mobPrefabs[1] : null;
            case EnemySlotType.Mob3:
                return mobPrefabs.Count > 2 ? mobPrefabs[2] : null;
            case EnemySlotType.Mob4:
                return mobPrefabs.Count > 3 ? mobPrefabs[3] : null;
            case EnemySlotType.Mob5:
                return mobPrefabs.Count > 4 ? mobPrefabs[4] : null;

            // Boss
            case EnemySlotType.Boss1:
                return bossPrefabs.Count > 0 ? bossPrefabs[0] : null;
            case EnemySlotType.Boss2:
                return bossPrefabs.Count > 1 ? bossPrefabs[1] : null;
            case EnemySlotType.Boss3:
                return bossPrefabs.Count > 2 ? bossPrefabs[2] : null;
            case EnemySlotType.Boss4:
                return bossPrefabs.Count > 3 ? bossPrefabs[3] : null;
            case EnemySlotType.Boss5:
                return bossPrefabs.Count > 4 ? bossPrefabs[4] : null;

            default:
                return null;
        }
    }

    // ====== MODIFIE CETTE FONCTION SELON TON SYSTÈME DE LEVEL ======
    public int CalculateLevel(float totalXP)
    {
        int level = 0;
        float xpNeeded = 2f;
        float xpNeededMultiplier = 1.5f;
        while (totalXP > xpNeeded)
        {
            level++;
            totalXP -= xpNeeded;
            xpNeeded *= xpNeededMultiplier;
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

    // ====== TRI MANUEL ======
#if UNITY_EDITOR
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
#endif
}