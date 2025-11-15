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

    [SerializeField] Shield shield;
    [SerializeField] TextMeshProUGUI m_shieldHpText;
    [SerializeField] Image m_shieldHpBar;

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
        }
        float amountHp = shield.Hp / shield.BaseHp;
        m_shieldHpBar.fillAmount = amountHp;
    }
}
