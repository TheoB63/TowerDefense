using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class EnemySpawnData
{
    [Tooltip("Le prefab de l'ennemi à spawn (doit avoir un script Enemy.cs)")]
    public GameObject enemyPrefab;

    [Tooltip("À quel moment spawn cet ennemi (en secondes depuis le début du level)")]
    public float spawnTime;

    // Récupère l'XP depuis le script Enemy du prefab
    public float GetXPValue()
    {
        if (enemyPrefab == null) return 0;
        Enemy enemyScript = enemyPrefab.GetComponent<Enemy>();
        if (enemyScript == null) return 0;
        return enemyScript.Xp;
    }

    // Récupère la couleur pour le preview
    public Color GetPreviewColor()
    {
        if (enemyPrefab == null) return Color.gray;
        Enemy enemyScript = enemyPrefab.GetComponent<Enemy>();
        if (enemyScript == null) return Color.white;
        return enemyScript.previewColor;
    }

    // Récupère le sprite pour l'affichage
    public Sprite GetSprite()
    {
        if (enemyPrefab == null) return null;
        SpriteRenderer spriteRenderer = enemyPrefab.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) return null;
        return spriteRenderer.sprite;
    }

    // Récupère le nom de l'ennemi
    public string GetEnemyName()
    {
        return enemyPrefab != null ? enemyPrefab.name : "NULL";
    }
}

[CreateAssetMenu(fileName = "Level", menuName = "TowerDefense/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Level Info")]
    public int levelNumber = 1;
    public string levelName = "Level 1";

    [Header("Enemies Timeline")]
    [Tooltip("Liste de tous les ennemis à spawn avec leur timing exact")]
    public List<EnemySpawnData> enemies = new List<EnemySpawnData>();

    [Header("Player Level Calculation")]
    [Tooltip("XP de base que le joueur a au début de ce level")]
    public float baseXP = 0;

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
            total += enemy.GetXPValue();
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

    // ====== TRI MANUEL ======
    // Clic droit sur le composant → "Sort Enemies by Time" pour trier
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
#endif
}