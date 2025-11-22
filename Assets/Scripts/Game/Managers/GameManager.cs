using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
        DataManager.Instance.playerManager.AddXp(15);
    }
}
