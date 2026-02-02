using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //Level
    LevelConfig m_currentLevel;

    //Time
    bool m_isPause;
    float m_levelTimer;
    float m_gameSpeed;

    //public references
    public bool IsPause { get => m_isPause; }
    public float LevelTimer { get => m_levelTimer; }

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

    private void Start()
    {
        //level
        m_currentLevel = DataManager.Instance.selectedLevel;

        //game speed
        SetGameSpeed(DataManager.Instance.gameSpeed);
    }

    void Update()
    {
        if (!m_isPause)
        {
            m_levelTimer += Time.deltaTime;
        }
    }

    public void PauseGame()
    {
        m_isPause = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        m_isPause = false;
        Time.timeScale = m_gameSpeed;
    }

    public void SetGameSpeed(float _speed)
    {
        m_gameSpeed = _speed;
        if (!m_isPause)
        {
            Time.timeScale = m_gameSpeed;
        }
    }

    public void EndGame(bool _win)
    {
        if (_win)
        {
            SceneManager.LoadScene("Menu");
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }

        AddRandomStuff();
    }

    void AddRandomStuff()
    {
        DataManager.Instance.playerManager.AddXp(m_currentLevel.xp);
        DataManager.Instance.inventoryManager.AddGold(m_currentLevel.gold);

        //Stuff
        int nbStuff = Random.Range(m_currentLevel.nbMinStuff, m_currentLevel.nbMaxStuff + 1);

        for (int i = 0; i < nbStuff; i++)
        {
            StuffReward selectReward = new StuffReward();
            float totalWeight = 0f;

            foreach (var stuff in m_currentLevel.stuffRewards)
            {
                totalWeight += stuff.chance;
            }

            float random = Random.Range(0, totalWeight);

            foreach (var stuff in m_currentLevel.stuffRewards)
            {
                if (random < stuff.chance)
                {
                    selectReward = stuff;
                    break;
                }

                random -= stuff.chance;
            }

            EquipmentType type = (EquipmentType)Random.Range(0, System.Enum.GetValues(typeof(EquipmentType)).Length);

            EquipmentItem newStuff = new EquipmentItem();
            newStuff.Type = type;
            newStuff.Rarity = selectReward.rarity;
            newStuff.Level = selectReward.stuffLevel;

            DataManager.Instance.inventoryManager.AddStuff(newStuff);
        }

        //Blueprints
        for (int i = 0; i < m_currentLevel.nbStuffBlueprints; i++)
        {
            EquipmentType type = (EquipmentType)Random.Range(0, System.Enum.GetValues(typeof(EquipmentType)).Length);
            DataManager.Instance.inventoryManager.AddStuffBlueprints(type, 1);
        }

        //Tower fragments
        for (int i = 0; i < m_currentLevel.nbTowerFragments; i++)
        {
            TowerType type = (TowerType)Random.Range(0, System.Enum.GetValues(typeof(TowerType)).Length);
            DataManager.Instance.inventoryManager.AddTowerFragments(type, 1);
        }
    }
}
