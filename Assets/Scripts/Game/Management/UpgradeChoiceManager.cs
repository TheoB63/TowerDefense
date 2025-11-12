using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeChoiceManager : MonoBehaviour
{
    [Header("Configuration")]
    public List<TowerConstructionUpgrade> allConstructionUpgrades;
    public List<TowerSpecificUpgrade> allTowerUpgrades;
    public List<PlayerUpgrade> allPlayerUpgrades;
    public int numberOfChoices = 3;

    [Header("References")]
    public PlayerStats playerStats;
    public List<TowerSlot> towerSlots;

    [Header("UI")]
    public GameObject upgradePanel;
    public List<Button> upgradeButtons; //need to change to upgradeButton
    public Button testLevelUpButton; // Pour tester

    private void Start()
    {
        if (upgradePanel != null) upgradePanel.SetActive(false);

        // Setup test button
        if (testLevelUpButton != null)
        {
            testLevelUpButton.onClick.AddListener(OnLevelUp);
        }
    }

    public void OnLevelUp()
    {
        Debug.Log("=== LEVEL UP ===");
        List<UpgradeBase> availableUpgrades = GetAvailableUpgrades();

        if (availableUpgrades.Count == 0)
        {
            Debug.LogWarning("Aucun upgrade disponible!");
            return;
        }

        List<UpgradeBase> randomChoices = SelectRandomUpgrades(availableUpgrades, numberOfChoices);
        ShowUpgradeUI(randomChoices);
    }

    private List<UpgradeBase> GetAvailableUpgrades()
    {
        List<UpgradeBase> available = new List<UpgradeBase>();

        // 1. Vérifier les constructions possibles
        foreach (var slot in towerSlots)
        {
            foreach (var construction in allConstructionUpgrades)
            {
                if (construction.CanApply(slot, playerStats))
                {
                    // Créer une copie avec le slot spécifique
                    var copy = Instantiate(construction);
                    //copy.specificSlot = slot;
                    available.Add(copy);
                    Debug.Log($"[Dispo] Construction: {construction.upgradeName} sur {slot.slotName}");
                }
            }
        }

        // 2. Vérifier les upgrades de tourelles existantes
        foreach (var slot in towerSlots)
        {
            if (slot.HasTower())
            {
                ITower tower = slot.GetTower();
                var availableTowerUpgrades = tower.GetAvailableUpgrades();

                foreach (var upgrade in allTowerUpgrades)
                {
                    if (availableTowerUpgrades.Contains(upgrade.upgradeMethodName) &&
                        upgrade.CanApply(slot, playerStats))
                    {
                        var copy = Instantiate(upgrade);
                        //copy.specificSlot = slot;
                        available.Add(copy);
                        Debug.Log($"[Dispo] Upgrade tourelle: {upgrade.upgradeName} pour {tower.GetTowerName()}");
                    }
                }
            }
        }

        // 3. Ajouter upgrades joueur
        foreach (var upgrade in allPlayerUpgrades)
        {
            if (upgrade.CanApply(null, playerStats))
            {
                available.Add(upgrade);
                Debug.Log($"[Dispo] Upgrade joueur: {upgrade.upgradeName}");
            }
        }

        return available;
    }

    private List<UpgradeBase> SelectRandomUpgrades(List<UpgradeBase> available, int count)
    {
        List<UpgradeBase> selected = new List<UpgradeBase>();
        List<UpgradeBase> pool = new List<UpgradeBase>(available);

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            selected.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }

        return selected;
    }

    private void ShowUpgradeUI(List<UpgradeBase> choices)
    {
        if (upgradePanel != null) upgradePanel.SetActive(true);

        // Arrêter le temps
        Time.timeScale = 0f;

        // Setup des boutons
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            if (i < choices.Count)
            {
                upgradeButtons[i].gameObject.SetActive(true);

                // Trouver le bon slot pour cet upgrade
                TowerSlot targetSlot = null;
                //if (choices[i] is TowerConstructionUpgrade constructionUpgrade)
                //{
                //    targetSlot = constructionUpgrade.specificSlot;
                //}
                //else if (choices[i] is TowerSpecificUpgrade towerUpgrade)
                //{
                //    targetSlot = towerUpgrade.specificSlot;
                //}

                //upgradeButtons[i].Setup(choices[i], targetSlot, this);
            }
            else
            {
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void ApplyUpgrade(UpgradeBase upgrade, TowerSlot targetSlot)
    {
        if (upgrade.CanApply(targetSlot, playerStats))
        {
            upgrade.Apply(targetSlot, playerStats);
            CloseUpgradeUI();
        }
        else
        {
            Debug.LogWarning("Impossible d'appliquer cet upgrade!");
        }
    }

    private void CloseUpgradeUI()
    {
        if (upgradePanel != null) upgradePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
