using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public Transform spawnPoint;

    [Header("Debug Information")] //need to delete later when all work
    [SerializeField] int enemiesSpawned;
    [SerializeField] int totalEnemiesInLevel;

    //Level
    LevelConfig currentLevel;

    //Enemies
    List<EnemySpawnData> m_remainingEnemies = new List<EnemySpawnData>();
    List<Enemy> m_enemies = new List<Enemy>();

    //Public
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
        if (DataManager.Instance == null)
        {
            return;
        }
        LoadLevel();
    }

    void Update()
    {
        SpawnEnemies();

        if (LevelIsEnd())
        {
            GameManager.Instance.EndGame(true);
        }
    }

    void LoadLevel()
    {
        currentLevel = DataManager.Instance.selectedLevel;

        if (currentLevel == null)
        {
            return;
        }

        if (spawnPoint == null)
        {
            return;
        }

        enemiesSpawned = 0;
        totalEnemiesInLevel = currentLevel.GetTotalEnemies();

        m_remainingEnemies = new List<EnemySpawnData>(currentLevel.enemies);
        m_remainingEnemies = m_remainingEnemies.OrderBy(e => e.spawnTime).ToList();
    }

    bool LevelIsEnd()
    {
        if (m_remainingEnemies.Count == 0 && m_enemies.Count == 0)
        {
            return true;
        }
        return false;
    }

    void SpawnEnemies()
    {
        for (int i = m_remainingEnemies.Count - 1; i >= 0; i--)
        {
            if (GameManager.Instance.LevelTimer >= m_remainingEnemies[i].spawnTime)
            {
                CreateEnemy(m_remainingEnemies[i]);
                m_remainingEnemies.Remove(m_remainingEnemies[i]);
                enemiesSpawned++;
            }
        }
    }

    void CreateEnemy(EnemySpawnData enemyData)
    {
        GameObject prefab = enemyData.GetPrefab(currentLevel);

        if (prefab == null)
        {
            return;
        }

        float x = Random.Range(0, Screen.width);
        Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(x, Screen.height, Camera.main.nearClipPlane));
        position.z = 0;
        position.y = spawnPoint.position.y;

        GameObject go = Instantiate(prefab, position, Quaternion.identity);
        Enemy enemy = go.GetComponent<Enemy>();

        if (enemy != null)
        {
            m_enemies.Add(enemy);
        }

        if (enemyData.IsBoss())
        {
            //create boss life bar here
        }
    }

    public Enemy GetNearestEnemy(Transform _transform)
    {
        if (m_enemies.Count == 0)
        {
            return null;
        }

        Enemy target = m_enemies[0];
        float dist = float.MaxValue;

        foreach (Enemy enemy in m_enemies)
        {
            float newDist = Vector2.Distance(_transform.position, enemy.transform.position);
            if (newDist < dist)
            {
                target = enemy;
                dist = newDist;
            }
        }
        return target;
    }

    public void StopLevel()
    {
        m_remainingEnemies.Clear();

        foreach (Enemy enemy in m_enemies)
        {
            Destroy(enemy.gameObject);
        }
        m_enemies.Clear();
    }

    public void RestartLevel()
    {
        StopLevel();
        LoadLevel();
    }
}