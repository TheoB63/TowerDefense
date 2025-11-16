using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    static public UI Instance;

    [Header("Level")]
    [SerializeField] TextMeshProUGUI m_levelText;
    [SerializeField] Image m_xpBar;

    [Header("Timer")]
    [SerializeField] TextMeshProUGUI timerText;

    [Header("Level")]
    [SerializeField] Shield shield;
    [SerializeField] TextMeshProUGUI m_shieldHpText;
    [SerializeField] Image m_shieldHpBar;

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

    private void Update()
    {
        timerText.text = FormatTime(EnemyManager.Instance.LevelTimer);
    }

    public void UpdateUI()
    {
        //level
        m_levelText.text = LevelManager.Instance.Level.ToString();
        float amountLvl = LevelManager.Instance.Xp / LevelManager.Instance.XpNeeded;
        m_xpBar.fillAmount = amountLvl;

        //shield hp
        if (shield != null)
        {
            m_shieldHpText.text = shield.Hp.ToString() + "/" + shield.BaseHp.ToString();
            float amountHp = shield.Hp / shield.BaseHp;
            m_shieldHpBar.fillAmount = amountHp;
        }
    }

    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return string.Format("{0}:{1:00}", minutes, secs);
    }

}
