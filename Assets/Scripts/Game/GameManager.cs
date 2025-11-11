using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //instance
    static public GameManager Instance;

    //Need to rework later
    #region Enemy     
    [Header("Enemies")]
    [SerializeField] List<GameObject> m_enemiesType;
    List<Enemy> m_enemies = new List<Enemy>();
    //spawn timer
    [SerializeField] float m_maxTimerEnemies;
    float m_timerEnemies;
    #endregion

    [Header("Public Values(Get only)")]
    public List<Enemy> Enemies { get => m_enemies; }

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

    void Start()
    {

    }

    void Update()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        m_timerEnemies += Time.deltaTime;

        if (m_timerEnemies > m_maxTimerEnemies)
        {
            foreach (GameObject enemy in m_enemiesType)
            {
                float x = Random.Range(0, Screen.width);
                Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(x, Screen.height, Camera.main.nearClipPlane));
                GameObject go = Instantiate(enemy, position, Quaternion.identity);
                m_enemies.Add(go.GetComponent<Enemy>());
            }
            m_timerEnemies -= m_maxTimerEnemies;
        }
    }
}
