using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] int m_startLevel;
    [SerializeField] int m_maxLevel;
    [SerializeField] TextMeshProUGUI levelText;

    int m_currentLevel;

    private void Start()
    {
        m_currentLevel = m_startLevel;
        UpdateText();
    }

    public void NextLevel()
    {
        m_currentLevel++;
        if (m_currentLevel > m_maxLevel)
        {
            m_currentLevel = m_maxLevel;
        }
        UpdateText();
    }

    public void PrevisousLevel()
    {
        m_currentLevel--;
        if (m_currentLevel < 1)
        {
            m_currentLevel = 1;
        }
        UpdateText();
    }

    void UpdateText()
    {
        levelText.text = "Level " + m_currentLevel;
    }

    public void StartGame()
    {
        DataManager.Instance.selectedLevel = m_currentLevel;
        SceneManager.LoadScene("Game");
    }
}
