using System.Collections.Generic;
using UnityEngine;

public class UpgradeChoiceManager : MonoBehaviour
{
    [Header("Upgrades")]
    [SerializeField] List<UpgradeBase> m_allUpgrades = new List<UpgradeBase>();

    [Header("References")]
    [SerializeField] PlayerStats m_playerStats;
    [SerializeField] List<TowerSlot> m_towerSlots;

    [Header("UI")]
    [SerializeField] GameObject m_upgradePanel;
    [SerializeField] List<UpgradeButton> m_upgradeButtons;

    void Start()
    {
        foreach (string upgrade in DataManager.Instance.unlockedGameUpgrades)
        {
            m_allUpgrades.Add(GameRegistry.Instance.GetUpgradeByName(upgrade));
        }

        if (m_upgradePanel != null)
        {
            m_upgradePanel.SetActive(false);
        }
    }

    public void OnLevelUp()
    {
        List<UpgradeBase> availableUpgrades = GetAvailableUpgrades();

        if (availableUpgrades.Count <= 0)
        {
            //This warning is normally impossible to attempt
            Debug.LogWarning("No upgrade Available !");
            return;
        }

        List<UpgradeBase> randomChoices = SelectRandomUpgrades(availableUpgrades, m_upgradeButtons.Count);
        ShowUpgradeUI(randomChoices);
    }

    List<UpgradeBase> GetAvailableUpgrades()
    {
        List<UpgradeBase> available = new List<UpgradeBase>();

        //Get all towers build 
        List<TowerType> existingTowerTypes = new List<TowerType>();
        foreach (TowerSlot slot in m_towerSlots)
        {
            if (slot.HasTower())
            {
                existingTowerTypes.Add(slot.Type);
            }
        }

        foreach (UpgradeBase upgrade in m_allUpgrades)
        {
            //Available build
            if (upgrade is TowerConstructionUpgrade constructionUpgrade)
            {
                if (!existingTowerTypes.Contains(constructionUpgrade.towerType))
                {
                    foreach (TowerSlot slot in m_towerSlots)
                    {
                        if (!slot.HasTower())
                        {
                            if (constructionUpgrade.CanApply(slot, m_playerStats))
                            {
                                bool isInList = false;
                                foreach (UpgradeBase up in available)
                                {
                                    if (constructionUpgrade.upgradeName == up.upgradeName)
                                    {
                                        isInList = true;
                                        break;
                                    }
                                }
                                if (!isInList)
                                {
                                    TowerConstructionUpgrade copy = Instantiate(constructionUpgrade);
                                    available.Add(copy);
                                }
                                break;
                            }
                        }
                    }
                }
            }

            //PlayerUpgrade
            else if (upgrade is PlayerUpgrade playerUpgrade)
            {
                //Need to rework when upgrade level and maxUgrade was created
                if (playerUpgrade.CanApply(null, m_playerStats))
                {
                    //verif if can upgrade with upgrade level and tiers

                    bool isInList = false;
                    foreach (UpgradeBase up in available)
                    {
                        if (playerUpgrade.upgradeName == up.upgradeName)
                        {
                            isInList = true;
                            break;
                        }
                    }
                    if (!isInList)
                    {
                        available.Add(playerUpgrade);
                    }
                }
            }

            //TowerUpgrade
            else if (upgrade is TowerUpgrade towerUpgrade)
            {
                //need to rework beacause dont work with m_allUgrade here
                foreach (TowerSlot slot in m_towerSlots)
                {
                    if (slot.HasTower() && slot.Type == towerUpgrade.targetTowerType)
                    {
                        ITower tower = slot.GetTower();
                        if (tower != null)
                        {
                            bool isInList = false;
                            foreach (UpgradeBase up in available)
                            {
                                if (towerUpgrade.upgradeName == up.upgradeName)
                                {
                                    isInList = true;
                                    break;
                                }
                            }
                            if (!isInList)
                            {
                                UpgradeBase copy = Instantiate(towerUpgrade);
                                available.Add(copy);
                            }
                            break;
                        }
                        break;
                    }
                }
            }
        }

        if (available.Count == 0)
        {
            //This warning is normally impossible to attempt
            Debug.LogWarning("No upgrade Available ! Add Player Upgrade to debug this !");
            foreach (UpgradeBase upgrade in m_allUpgrades)
            {
                if (upgrade is PlayerUpgrade playerUpgrade)
                {
                    available.Add(playerUpgrade);
                }
            }
        }

        return available;
    }

    List<UpgradeBase> SelectRandomUpgrades(List<UpgradeBase> _available, int _count)
    {
        //Need to rework for add Values (% chance to find this upgrade)
        //Code not clean need it

        List<UpgradeBase> selected = new List<UpgradeBase>();
        List<UpgradeBase> pool = new List<UpgradeBase>(_available);

        while (selected.Count < _count && pool.Count > 0)
        {
            int randomIndex = Random.Range(0, pool.Count);
            selected.Add(pool[randomIndex]);
            pool.Remove(pool[randomIndex]);
        }

        return selected;
    }

    void ShowUpgradeUI(List<UpgradeBase> _choices)
    {
        if (m_upgradePanel != null) m_upgradePanel.SetActive(true);

        GameManager.Instance.PauseGame();

        //Setup Buttons
        for (int i = 0; i < m_upgradeButtons.Count; i++)
        {
            if (i < _choices.Count)
            {
                m_upgradeButtons[i].gameObject.SetActive(true);

                //Find slot for this upgrade
                TowerSlot targetSlot = null;
                if (_choices[i] is TowerConstructionUpgrade constructionUpgrade)
                {
                    foreach (TowerSlot slot in m_towerSlots)
                    {
                        if (!slot.HasTower())
                        {
                            targetSlot = slot;
                            break;
                        }
                    }
                }
                else if (_choices[i] is TowerUpgrade towerUpgrade)
                {
                    foreach (TowerSlot slot in m_towerSlots)
                    {
                        if (slot.Type == towerUpgrade.targetTowerType)
                        {
                            targetSlot = slot;
                            break;
                        }
                    }
                }

                m_upgradeButtons[i].Setup(_choices[i], targetSlot, this);
            }
            else
            {
                m_upgradeButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void ApplyUpgrade(UpgradeBase _upgrade, TowerSlot _targetSlot)
    {
        if (_upgrade.CanApply(_targetSlot, m_playerStats))
        {
            _upgrade.Apply(_targetSlot, m_playerStats);
            CloseUpgradeUI();
        }
        else
        {
            //This warning is normally impossible to attempt
            Debug.LogWarning("Impossible to apply upgrade!");
        }
    }

    void CloseUpgradeUI()
    {
        if (m_upgradePanel != null) m_upgradePanel.SetActive(false);

        GameManager.Instance.ResumeGame();
    }
}
