using System.Collections.Generic;
using System.Linq;
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
    public List<UpgradeButton> upgradeButtons;
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

        // 1. Obtenir les types de tourelles déjà présentes
        HashSet<TowerType> existingTowerTypes = new HashSet<TowerType>();
        foreach (var slot in towerSlots)
        {
            if (slot.HasTower())
            {
                existingTowerTypes.Add(slot.type);
            }
        }

        // 2. Constructions possibles (éviter les doublons de type)
        foreach (var slot in towerSlots)
        {
            if (!slot.HasTower()) // Slot vide
            {
                foreach (var construction in allConstructionUpgrades)
                {
                    // Vérifier qu'on n'a pas déjà ce type de tourelle
                    if (!existingTowerTypes.Contains(construction.towerType) &&
                        construction.CanApply(slot, playerStats))
                    {
                        var copy = Instantiate(construction);
                        available.Add(copy);
                        Debug.Log($"[Dispo] Construction: {construction.upgradeName} (type {construction.towerType})");
                    }
                }
            }
        }

        // 3. Upgrades de tourelles EXISTANTES uniquement
        foreach (var slot in towerSlots)
        {
            if (slot.HasTower())
            {
                ITower tower = slot.GetTower();
                if (tower == null) continue;

                var availableTowerUpgrades = tower.GetAvailableUpgrades();

                foreach (var upgrade in allTowerUpgrades)
                {
                    // Vérifier que c'est le bon type ET que l'upgrade est dispo
                    if (upgrade.targetTowerType == slot.type &&
                        availableTowerUpgrades.Contains(upgrade.upgradeMethodName) &&
                        upgrade.CanApply(slot, playerStats))
                    {
                        var copy = Instantiate(upgrade);
                        available.Add(copy);
                        Debug.Log($"[Dispo] Upgrade tourelle: {upgrade.upgradeName} pour {tower.GetTowerName()}");
                    }
                }
            }
        }

        // 4. Upgrades joueur (toujours disponibles si on a l'argent)
        foreach (var upgrade in allPlayerUpgrades)
        {
            if (upgrade.CanApply(null, playerStats))
            {
                available.Add(upgrade);
                Debug.Log($"[Dispo] Upgrade joueur: {upgrade.upgradeName}");
            }
        }

        // 5. SÉCURITÉ : Si vraiment aucun choix, forcer des upgrades joueur gratuits
        if (available.Count == 0)
        {
            Debug.LogWarning("⚠️ Aucun upgrade disponible! Ajout d'un bonus gratuit...");
            // Tu peux créer un upgrade spécial "Bonus Currency" sans coût
            foreach (var upgrade in allPlayerUpgrades)
            {
                available.Add(upgrade); // Ajouter même si pas assez d'argent
            }
        }

        return available;
    }

    private List<UpgradeBase> SelectRandomUpgrades(List<UpgradeBase> available, int count)
    {
        List<UpgradeBase> selected = new List<UpgradeBase>();
        List<UpgradeBase> pool = new List<UpgradeBase>(available);

        // Prioriser la diversité des choix
        // Séparer par type pour éviter d'avoir 3 fois le même upgrade
        var constructions = pool.OfType<TowerConstructionUpgrade>().ToList();
        var towerUpgrades = pool.OfType<TowerSpecificUpgrade>().ToList();
        var playerUpgrades = pool.OfType<PlayerUpgrade>().ToList();

        // Essayer d'avoir au moins 1 de chaque type si possible
        List<List<UpgradeBase>> categories = new List<List<UpgradeBase>>
        {
            constructions.Cast<UpgradeBase>().ToList(),
            towerUpgrades.Cast<UpgradeBase>().ToList(),
            playerUpgrades.Cast<UpgradeBase>().ToList()
        };

        // Mélanger les catégories
        categories = categories.OrderBy(x => Random.value).ToList();

        // Prendre un de chaque catégorie d'abord
        foreach (var category in categories)
        {
            if (selected.Count >= count) break;
            if (category.Count > 0)
            {
                int randomIndex = Random.Range(0, category.Count);
                selected.Add(category[randomIndex]);
                category.RemoveAt(randomIndex);
            }
        }

        // Compléter avec le reste aléatoirement
        var remaining = constructions.Cast<UpgradeBase>()
            .Concat(towerUpgrades.Cast<UpgradeBase>())
            .Concat(playerUpgrades.Cast<UpgradeBase>())
            .ToList();

        while (selected.Count < count && remaining.Count > 0)
        {
            int randomIndex = Random.Range(0, remaining.Count);
            selected.Add(remaining[randomIndex]);
            remaining.RemoveAt(randomIndex);
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
                if (choices[i] is TowerConstructionUpgrade constructionUpgrade)
                {
                    foreach (TowerSlot slot in towerSlots)
                    {
                        if (!slot.HasTower())
                        {
                            targetSlot = slot;
                            break;
                        }
                    }
                }
                else if (choices[i] is TowerSpecificUpgrade towerUpgrade)
                {
                    foreach (TowerSlot slot in towerSlots)
                    {
                        if (slot.type == towerUpgrade.targetTowerType)
                        {
                            targetSlot = slot;
                            break;
                        }
                    }
                }

                upgradeButtons[i].Setup(choices[i], targetSlot, this);
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
