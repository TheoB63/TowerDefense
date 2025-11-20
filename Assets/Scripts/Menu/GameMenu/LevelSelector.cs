using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class LevelSelector : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_levelText;
    [SerializeField] List<LevelConfig> m_levels = new List<LevelConfig>();

    LevelConfig m_currentLevel;
    int m_currentLevelIndex;

    private void Start()
    {
        if (m_levels.Count > 0)
        {
            m_currentLevel = m_levels[0];
            m_currentLevelIndex = 0;
        }
        UpdateText();
    }

    public void NextLevel()
    {
        m_currentLevelIndex++;
        if (m_currentLevelIndex >= m_levels.Count)
        {
            m_currentLevelIndex = m_levels.Count - 1;
        }
        m_currentLevel = m_levels[m_currentLevelIndex];
        UpdateText();
    }

    public void PrevisousLevel()
    {
        m_currentLevelIndex--;
        if (m_currentLevelIndex < 0)
        {
            m_currentLevelIndex = 0;
        }
        m_currentLevel = m_levels[m_currentLevelIndex];
        UpdateText();
    }

    void UpdateText()
    {
        m_levelText.text = m_currentLevel.name;
    }

    public void StartGame()
    {
        //test lvl up if launch game
        DataManager.Instance.playerManager.AddXp(15);

        DataManager.Instance.selectedLevel = m_currentLevel;
        SceneManager.LoadScene("Game");
    }
}
