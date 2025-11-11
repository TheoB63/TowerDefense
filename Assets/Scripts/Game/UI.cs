using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    static public UI Instance;

    [Header("Level")]
    [SerializeField] TextMeshProUGUI m_levelText;
    [SerializeField] Image m_xpBar;

    void Start()
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

    public void UpdateUI()
    {
        m_levelText.text = LevelManager.Instance.Level.ToString();

        float amount = LevelManager.Instance.Xp / LevelManager.Instance.XpNeeded;
        m_xpBar.fillAmount = amount;
        Debug.Log(amount);
    }
}
