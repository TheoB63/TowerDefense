using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [Header("Configuration")]
    [Tooltip("Glisse tous tes LevelConfig ici (Level1, Level2, etc.)")]
    public List<LevelConfig> allLevels = new List<LevelConfig>();

    [Tooltip("Le Transform où spawner les ennemis (en haut de l'écran)")]
    public Transform spawnPoint;

    [Header("Debug Info - READ ONLY")]
    [SerializeField] private int currentLevelNumber;
    [SerializeField] private float levelTimer;
    [SerializeField] private int enemiesSpawned;
    [SerializeField] private int totalEnemiesInLevel;
    [SerializeField] private bool isLevelActive = false;

    private LevelConfig currentLevel;
    private List<EnemySpawnData> remainingEnemies = new List<EnemySpawnData>();

    List<Enemy> m_enemies = new List<Enemy>();
    public List<Enemy> Enemies { get => m_enemies; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Vérification du DataManager
        if (DataManager.Instance == null)
        {
            Debug.LogError("❌ DataManager.Instance is null! Make sure you have a DataManager in your Menu scene.");
            return;
        }

        int selectedLevel = DataManager.Instance.selectedLevel;
        LoadLevel(selectedLevel);
    }

    void Update()
    {
        if (!isLevelActive) return;

        // Incrémenter le timer
        levelTimer += Time.deltaTime;

        // Spawn tous les ennemis dont le timing est dépassé
        SpawnScheduledEnemies();

        // Vérifier si le level est terminé
        if (remainingEnemies.Count == 0 && enemiesSpawned >= totalEnemiesInLevel)
        {
            //OnLevelCompleted();
        }

        EndLevel();
    }

    // ========== FONCTION PRINCIPALE : CHARGER UN LEVEL ==========
    void LoadLevel(int levelNumber)
    {
        // Trouver le LevelConfig correspondant
        currentLevel = allLevels.Find(l => l.levelNumber == levelNumber);

        if (currentLevel == null)
        {
            Debug.LogError($"❌ Level {levelNumber} not found in allLevels list!");
            Debug.LogWarning($"Available levels: {string.Join(", ", allLevels.Select(l => l.levelNumber))}");
            return;
        }

        // Vérifier que le spawn point existe
        if (spawnPoint == null)
        {
            Debug.LogError("❌ Spawn Point is not assigned in EnemyManager!");
            return;
        }

        // Initialiser le level
        currentLevelNumber = levelNumber;
        levelTimer = 0f;
        enemiesSpawned = 0;
        totalEnemiesInLevel = currentLevel.GetTotalEnemies();

        // Copier tous les ennemis dans la liste de spawn (triés par temps)
        remainingEnemies = new List<EnemySpawnData>(currentLevel.enemies);
        remainingEnemies = remainingEnemies.OrderBy(e => e.spawnTime).ToList();

        // Activer le level
        isLevelActive = true;

        // Logs d'informations
        Debug.Log($"🎮 Starting {currentLevel.levelName}");
        Debug.Log($"📊 {totalEnemiesInLevel} enemies ({currentLevel.GetMobCount()} mobs, {currentLevel.GetBossCount()} bosses), {currentLevel.GetTotalXP()} XP total");
        Debug.Log($"🎯 Player Level progression: {currentLevel.GetStartLevel()} → {currentLevel.GetEndLevel()}");
        Debug.Log($"⏱️ Estimated duration: {currentLevel.GetLevelDuration():F1} seconds");
    }

    void EndLevel()
    {
        Debug.Log("Enemy end : remain=" + remainingEnemies.Count + " count=" + m_enemies.Count);

        if (remainingEnemies.Count == 0)
        {
            if (m_enemies.Count == 0)
            {
                GameManager.Instance.EndLevel(true);
            }
        }
    }

    // ========== SPAWN DES ENNEMIS AU BON TIMING ==========
    void SpawnScheduledEnemies()
    {
        // Parcourir la liste à l'envers pour pouvoir supprimer pendant l'itération
        for (int i = remainingEnemies.Count - 1; i >= 0; i--)
        {
            EnemySpawnData enemyData = remainingEnemies[i];

            // Si le timing est atteint ou dépassé, spawn l'ennemi
            if (levelTimer >= enemyData.spawnTime)
            {
                SpawnEnemy(enemyData);
                remainingEnemies.RemoveAt(i);
                enemiesSpawned++;
            }
        }
    }

    // ========== INSTANCIER UN ENNEMI ==========
    void SpawnEnemy(EnemySpawnData enemyData)
    {
        GameObject prefab = enemyData.GetPrefab(currentLevel);

        if (prefab == null)
        {
            Debug.LogWarning($"⚠️ No prefab assigned for {enemyData.GetEnemyName(currentLevel)} in {currentLevel.levelName}!");
            return;
        }

        // Instancier l'ennemi au spawn point
        float x = Random.Range(0, Screen.width);
        Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(x, Screen.height, Camera.main.nearClipPlane));
        position.z = 0;

        GameObject enemy = Instantiate(prefab, position, Quaternion.identity);
        Enemy enemyScript = enemy.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            m_enemies.Add(enemyScript);
        }

        // L'XP est déjà défini dans le prefab Enemy.cs, pas besoin de le setter !

        // Vérifier si c'est un boss pour des events spéciaux
        bool isBoss = enemyData.IsBoss();
        if (isBoss)
        {
            Debug.Log($"💀 BOSS SPAWNED: {enemyData.GetEnemyName(currentLevel)} ({enemyData.GetEnemyName(currentLevel)}) at {levelTimer:F1}s!");
            // Tu peux ajouter un event ici : OnBossSpawned?.Invoke(enemyScript);
        }
        else
        {
            Debug.Log($"👾 Spawned {enemyData.GetEnemyName(currentLevel)} ({enemyData.GetEnemyName(currentLevel)}) at {levelTimer:F1}s (worth {enemyData.GetXPValue(currentLevel)} XP)");
        }
    }

    // ========== FIN DU LEVEL ==========
    void OnLevelCompleted()
    {
        isLevelActive = false;

        Debug.Log("🎉 Level Complete!");
        Debug.Log($"⏱️ Total time: {levelTimer:F1}s");
        Debug.Log($"👾 Enemies spawned: {enemiesSpawned}/{totalEnemiesInLevel}");

        // Ici tu peux :
        // - Afficher un écran de victoire
        // - Donner les récompenses
        // - Charger le level suivant
        // - Retourner au menu

        // Exemple : charger le level suivant après 3 secondes
        // StartCoroutine(LoadNextLevelAfterDelay(3f));
    }

    // ========== FONCTIONS UTILES ==========
    public Enemy GetNearestEnemy(Transform _transform)
    {
        if (m_enemies.Count == 0) return null;

        Debug.Log("nb enemies get : " + m_enemies.Count);

        Enemy target = m_enemies[0];

        float dist = float.MaxValue;
        foreach (Enemy enemy in m_enemies)
        {
            if (enemy == null) continue; // Sécurité si l'ennemi est détruit

            float newDist = Vector2.Distance(_transform.position, enemy.transform.position);
            if (newDist < dist)
            {
                target = enemy;
                dist = newDist;
            }
        }
        return target;
    }

    // Arrêter complètement le level
    public void StopLevel()
    {
        isLevelActive = false;
        remainingEnemies.Clear();

        // Détruire tous les ennemis restants
        foreach (Enemy enemy in m_enemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }
        m_enemies.Clear();

        Debug.Log("🛑 Level stopped");
    }

    // Mettre en pause le spawn (les ennemis déjà spawned continuent)
    public void PauseLevel()
    {
        isLevelActive = false;
        Debug.Log("⏸️ Level paused");
    }

    // Reprendre le spawn
    public void ResumeLevel()
    {
        isLevelActive = true;
        Debug.Log("▶️ Level resumed");
    }

    // Charger le level suivant
    public void LoadNextLevel()
    {
        int nextLevel = currentLevelNumber + 1;

        if (allLevels.Any(l => l.levelNumber == nextLevel))
        {
            LoadLevel(nextLevel);
        }
        else
        {
            Debug.Log("🏆 No more levels! Game completed!");
        }
    }

    // Restart le level actuel
    public void RestartLevel()
    {
        // Nettoyer d'abord
        StopLevel();

        // Recharger
        LoadLevel(currentLevelNumber);
    }

    // ========== COROUTINE UTILE ==========
    IEnumerator LoadNextLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadNextLevel();
    }

    // ========== PREVIEW DANS L'ÉDITEUR (sans lancer le jeu) ==========
    [ContextMenu("Preview Level 1")]
    void PreviewLevel1()
    {
        PreviewLevel(1);
    }

    [ContextMenu("Preview Current Level")]
    void PreviewCurrentLevel()
    {
        if (allLevels.Count > 0)
        {
            PreviewLevel(allLevels[0].levelNumber);
        }
    }

    void PreviewLevel(int levelNumber)
    {
        LevelConfig level = allLevels.Find(l => l.levelNumber == levelNumber);

        if (level == null)
        {
            Debug.LogWarning($"Level {levelNumber} not found!");
            return;
        }

        Debug.Log($"========== LEVEL {level.levelNumber}: {level.levelName} ==========");
        Debug.Log($"Total Enemies: {level.GetTotalEnemies()} ({level.GetMobCount()} mobs, {level.GetBossCount()} bosses)");
        Debug.Log($"Total XP: {level.GetTotalXP()}");
        Debug.Log($"Duration: ~{level.GetLevelDuration():F1}s");
        Debug.Log($"Player Level: {level.GetStartLevel()} → {level.GetEndLevel()}");
        Debug.Log("");

        var sortedEnemies = level.enemies.OrderBy(e => e.spawnTime).ToList();
        foreach (var enemy in sortedEnemies)
        {
            string bossTag = enemy.IsBoss() ? " 💀 BOSS" : "";
            Debug.Log($"  {enemy.spawnTime:F1}s → {enemy.GetEnemyName(currentLevel)} ({enemy.GetEnemyName(level)}) - {enemy.GetXPValue(level)} XP{bossTag}");
        }
        Debug.Log("====================================");
    }
}