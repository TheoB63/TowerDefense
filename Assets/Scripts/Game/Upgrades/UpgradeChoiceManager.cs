using System.Collections.Generic;
using UnityEngine;

public class UpgradeChoiceManager : MonoBehaviour
{
    [Header("Upgrades")]
    [SerializeField] List<TowerConstructionUpgrade> m_constructionUpgrades;
    [SerializeField] List<PlayerUpgrade> m_playerUpgrades;

    [Header("References")]
    [SerializeField] PlayerStats m_playerStats;
    [SerializeField] List<TowerSlot> m_towerSlots;

    [Header("UI")]
    [SerializeField] GameObject m_upgradePanel;
    [SerializeField] List<UpgradeButton> m_upgradeButtons;

    void Start()
    {
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

        //Available build
        foreach (TowerSlot slot in m_towerSlots)
        {
            if (!slot.HasTower())
            {
                foreach (TowerConstructionUpgrade construction in m_constructionUpgrades)
                {
                    if (!existingTowerTypes.Contains(construction.towerType) &&
                        construction.CanApply(slot, m_playerStats))
                    {
                        bool isInList = false;
                        foreach (UpgradeBase up in available)
                        {
                            if (construction.upgradeName == up.upgradeName)
                            {
                                isInList = true;
                            }
                        }
                        if (!isInList)
                        {
                            TowerConstructionUpgrade copy = Instantiate(construction);
                            available.Add(copy);
                        }
                    }
                }
            }
        }

        //Upgrades Tower
        foreach (var slot in m_towerSlots)
        {
            if (slot.HasTower())
            {
                ITower tower = slot.GetTower();
                if (tower != null)
                {
                    List<UpgradeBase> availableTowerUpgrades = tower.GetAvailableUpgrades();

                    foreach (UpgradeBase upgrade in availableTowerUpgrades)
                    {
                        bool isInList = false;
                        foreach (UpgradeBase up in available)
                        {
                            if (upgrade.upgradeName == up.upgradeName)
                            {
                                isInList = true;
                            }
                        }
                        if (!isInList)
                        {
                            UpgradeBase copy = Instantiate(upgrade);
                            available.Add(copy);
                        }
                    }
                }
            }
        }

        //Player Upgrades
        foreach (PlayerUpgrade upgrade in m_playerUpgrades)
        {
            List<string> availablePlayerUpgrades = m_playerStats.GetAvailableUpgrades();
            if (availablePlayerUpgrades.Contains(upgrade.upgradeMethodName) &&
                upgrade.CanApply(null, m_playerStats))
            {
                bool isInList = false;
                foreach (UpgradeBase up in available)
                {
                    if (upgrade.upgradeName == up.upgradeName)
                    {
                        isInList = true;
                    }
                }
                if (!isInList)
                {
                    available.Add(upgrade);
                }
            }
        }

        if (available.Count == 0)
        {
            //This warning is normally impossible to attempt
            Debug.LogWarning("No upgrade Available ! Add Player Upgrade to debug this !");
            foreach (PlayerUpgrade upgrade in m_playerUpgrades)
            {
                available.Add(upgrade);
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

        //-------

        // Prioriser la diversité des choix
        // Séparer par type pour éviter d'avoir 3 fois le même upgrade
        //var constructions = pool.OfType<TowerConstructionUpgrade>().ToList();
        //var towerUpgrades = pool.OfType<TowerSpecificUpgrade>().ToList();
        //var playerUpgrades = pool.OfType<PlayerUpgrade>().ToList();

        // Essayer d'avoir au moins 1 de chaque type si possible
        //List<List<UpgradeBase>> categories = new List<List<UpgradeBase>>
        //{
        //    constructions.Cast<UpgradeBase>().ToList(),
        //    towerUpgrades.Cast<UpgradeBase>().ToList(),
        //    playerUpgrades.Cast<UpgradeBase>().ToList()
        //};

        //// Mélanger les catégories
        //categories = categories.OrderBy(x => Random.value).ToList();

        //// Prendre un de chaque catégorie d'abord
        //foreach (var category in categories)
        //{
        //    if (selected.Count >= _count) break;
        //    if (category.Count > 0)
        //    {
        //        int randomIndex = Random.Range(0, category.Count);
        //        selected.Add(category[randomIndex]);
        //        category.Remove(category[randomIndex]);
        //    }
        //}

        //----

        // Compléter avec le reste aléatoirement
        //var remaining = constructions.Cast<UpgradeBase>()
        //    .Concat(towerUpgrades.Cast<UpgradeBase>())
        //    .Concat(playerUpgrades.Cast<UpgradeBase>())
        //    .ToList();

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

        //Stop Time but isn't a good way, need to see other method
        Time.timeScale = 0f;

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
        //Same as stop time, need to find other solution
        Time.timeScale = 1f;
    }
}
