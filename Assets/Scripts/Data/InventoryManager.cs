using System;
using System.Collections.Generic;
using UnityEngine;

public enum StuffType
{
    Helmet,
    Plate,
    Legs,
    Boots,
    Shield,
    Canon
}

[System.Serializable]
public class StuffItem
{
    public StuffType Type;
    public int Level;
    public int Attack;
    public int Defense;

    public StuffItem(StuffType type, int level = 1)
    {
        Type = type;
        Level = level;
    }
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    //Basic
    public int gold;
    public int diamond;

    //Stuff
    [SerializeField] private List<StuffItem> ownedStuff = new List<StuffItem>();
    private Dictionary<StuffType, StuffItem> equippedStuff = new Dictionary<StuffType, StuffItem>();

    //Fragments upgrade
    private Dictionary<TowerType, int> towerFragments = new Dictionary<TowerType, int>();
    private Dictionary<StuffType, int> stuffBlueprints = new Dictionary<StuffType, int>();

    //Upgrades


    //Events
    public event Action<int> OnGoldChanged;
    public event Action<int> OnDiamondChanged;
    public event Action<StuffType, StuffItem> OnStuffEquipped;

    void Awake()
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

    // ========== GOLD ==========
    public void AddGold(int amount)
    {
        if (amount < 0) return;
        gold += amount;
        OnGoldChanged?.Invoke(gold);
    }

    public bool SpendGold(int amount)
    {
        if (amount < 0 || gold < amount) return false;
        gold -= amount;
        OnGoldChanged?.Invoke(gold);
        return true;
    }

    // ========== DIAMOND ==========
    public void AddDiamond(int amount)
    {
        if (amount < 0) return;
        diamond += amount;
        OnDiamondChanged?.Invoke(diamond);
    }

    public bool SpendDiamond(int amount)
    {
        if (amount < 0 || diamond < amount) return false;
        diamond -= amount;
        OnDiamondChanged?.Invoke(diamond);
        return true;
    }

    // ========== STUFF ==========
    public void AddStuff(StuffItem item)
    {
        if (item == null) return;
        ownedStuff.Add(item);
        Debug.Log($"Added {item.Type} (Level {item.Level}) to inventory");
    }

    public void RemoveStuff(StuffItem item)
    {
        ownedStuff.Remove(item);
    }

    public List<StuffItem> GetOwnedStuff()
    {
        return new List<StuffItem>(ownedStuff); // Retourne une copie
    }

    public List<StuffItem> GetStuffByType(StuffType type)
    {
        return ownedStuff.FindAll(s => s.Type == type);
    }

    // ========== EQUIPPED STUFF ==========
    public void EquipStuff(StuffItem item)
    {
        if (item == null) return;

        // Si un item est déjà équipé, le retirer
        if (equippedStuff.ContainsKey(item.Type))
        {
            var previousItem = equippedStuff[item.Type];
            // Optionnel : remettre dans l'inventaire si ce n'est pas déjà dedans
            if (!ownedStuff.Contains(previousItem))
            {
                ownedStuff.Add(previousItem);
            }
        }

        equippedStuff[item.Type] = item;
        ownedStuff.Remove(item); // Retire de l'inventaire non équipé
        OnStuffEquipped?.Invoke(item.Type, item);
        Debug.Log($"Equipped {item.Type} (Level {item.Level})");
    }

    public void UnequipStuff(StuffType type)
    {
        if (equippedStuff.TryGetValue(type, out StuffItem item))
        {
            equippedStuff.Remove(type);
            ownedStuff.Add(item); // Remet dans l'inventaire
            OnStuffEquipped?.Invoke(type, null);
            Debug.Log($"Unequipped {type}");
        }
    }

    public StuffItem GetEquippedStuff(StuffType type)
    {
        return equippedStuff.ContainsKey(type) ? equippedStuff[type] : null;
    }

    public bool HasEquippedStuff(StuffType type)
    {
        return equippedStuff.ContainsKey(type);
    }

    public Dictionary<StuffType, StuffItem> GetAllEquippedStuff()
    {
        return new Dictionary<StuffType, StuffItem>(equippedStuff); // Retourne une copie
    }

    // ========== TOWER FRAGMENTS ==========
    public void AddTowerFragments(TowerType type, int amount)
    {
        if (amount < 0) return;

        if (!towerFragments.ContainsKey(type))
        {
            towerFragments[type] = 0;
        }
        towerFragments[type] += amount;
        Debug.Log($"Added {amount} {type} fragments. Total: {towerFragments[type]}");
    }

    public int GetTowerFragments(TowerType type)
    {
        return towerFragments.TryGetValue(type, out int count) ? count : 0;
    }

    public bool SpendTowerFragments(TowerType type, int amount)
    {
        if (amount < 0) return false;

        if (towerFragments.TryGetValue(type, out int current) && current >= amount)
        {
            towerFragments[type] -= amount;
            Debug.Log($"Spent {amount} {type} fragments. Remaining: {towerFragments[type]}");
            return true;
        }

        Debug.LogWarning($"Not enough {type} fragments. Need {amount}, have {current}");
        return false;
    }

    public bool CanUpgradeTower(TowerType type, int fragmentsRequired)
    {
        return GetTowerFragments(type) >= fragmentsRequired;
    }

    // ========== STUFF BLUEPRINTS ==========
    public void AddStuffBlueprints(StuffType type, int amount)
    {
        if (amount < 0) return;

        if (!stuffBlueprints.ContainsKey(type))
        {
            stuffBlueprints[type] = 0;
        }
        stuffBlueprints[type] += amount;
        Debug.Log($"Added {amount} {type} blueprints. Total: {stuffBlueprints[type]}");
    }

    public int GetStuffBlueprints(StuffType type)
    {
        return stuffBlueprints.TryGetValue(type, out int count) ? count : 0;
    }

    public bool SpendStuffBlueprints(StuffType type, int amount)
    {
        if (amount < 0) return false;

        if (stuffBlueprints.TryGetValue(type, out int current) && current >= amount)
        {
            stuffBlueprints[type] -= amount;
            Debug.Log($"Spent {amount} {type} blueprints. Remaining: {stuffBlueprints[type]}");
            return true;
        }

        Debug.LogWarning($"Not enough {type} blueprints. Need {amount}, have {current}");
        return false;
    }

    public bool CanUpgradeStuff(StuffType type, int blueprintsRequired)
    {
        return GetStuffBlueprints(type) >= blueprintsRequired;
    }

    // ========== UTILITY ==========
    public void ResetInventory()
    {
        gold = 0;
        diamond = 0;
        ownedStuff.Clear();
        equippedStuff.Clear();
        towerFragments.Clear();
        stuffBlueprints.Clear();
        Debug.Log("Inventory reset");
    }

    // Debug : afficher tout l'inventaire
    public void LogInventory()
    {
        Debug.Log("===== INVENTORY =====");
        Debug.Log($"Gold: {gold}");
        Debug.Log($"Diamond: {diamond}");
        Debug.Log($"Owned Stuff: {ownedStuff.Count}");
        Debug.Log($"Equipped Stuff: {equippedStuff.Count}");
        Debug.Log($"Tower Fragments Types: {towerFragments.Count}");
        Debug.Log($"Stuff Blueprints Types: {stuffBlueprints.Count}");
    }
}
