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
    }

    public void ShowUpgradeChoices(List<UpgradeTower> upgrades)
    {
        Debug.Log("=== Choix d'upgrade ===");

        for (int i = 0; i < upgrades.Count; i++)
        {
            Debug.Log($"{i + 1}. {upgrades[i].upgradeName} ({upgrades[i].category})");
        }

        if (upgrades.Count > 0)
        {
            foreach (UpgradeTower item in upgrades)
            {
                if (true)
                {
                    item.requireTowerBuilt = false;
                }
                //item.ApplyUpgrade(FindFirstObjectByType<TowerManager>());
            }

            //upgrades[0].ApplyUpgrade(FindFirstObjectByType<TowerManager>());
        }
    }
}
