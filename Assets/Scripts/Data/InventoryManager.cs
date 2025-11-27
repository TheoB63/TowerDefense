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
    private int m_gold;
    private int m_diamond;

    //Stuff
    [SerializeField] private List<StuffItem> m_ownedStuff = new List<StuffItem>();
    private Dictionary<StuffType, StuffItem> m_equippedStuff = new Dictionary<StuffType, StuffItem>();

    //Fragments upgrade
    private Dictionary<TowerType, int> m_towerFragments = new Dictionary<TowerType, int>();
    private Dictionary<StuffType, int> m_stuffBlueprints = new Dictionary<StuffType, int>();

    //Upgrades


    //Events
    public event Action<int> OnGoldChanged;
    public event Action<int> OnDiamondChanged;
    public event Action<StuffType, StuffItem> OnStuffEquipped;

    //Public
    public int Gold { get => m_gold; }
    public int Diamond { get => m_diamond; }

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

    private void Start()
    {
        AddTowerFragments(TowerType.Basic, 400);
        AddGold(40000);
    }

    // ========== GOLD ==========
    public void AddGold(int amount)
    {
        if (amount < 0) return;
        m_gold += amount;
        OnGoldChanged?.Invoke(m_gold);
    }

    public bool SpendGold(int amount)
    {
        if (amount < 0 || m_gold < amount) return false;
        m_gold -= amount;
        OnGoldChanged?.Invoke(m_gold);
        return true;
    }

    // ========== DIAMOND ==========
    public void AddDiamond(int amount)
    {
        if (amount < 0) return;
        m_diamond += amount;
        OnDiamondChanged?.Invoke(m_diamond);
    }

    public bool SpendDiamond(int amount)
    {
        if (amount < 0 || m_diamond < amount) return false;
        m_diamond -= amount;
        OnDiamondChanged?.Invoke(m_diamond);
        return true;
    }

    // ========== STUFF ==========
    public void AddStuff(StuffItem item)
    {
        if (item == null) return;
        m_ownedStuff.Add(item);
    }

    public void RemoveStuff(StuffItem item)
    {
        m_ownedStuff.Remove(item);
    }

    public List<StuffItem> GetOwnedStuff()
    {
        return new List<StuffItem>(m_ownedStuff); // Retourne une copie
    }

    public List<StuffItem> GetStuffByType(StuffType type)
    {
        return m_ownedStuff.FindAll(s => s.Type == type);
    }

    // ========== EQUIPPED STUFF ==========
    public void EquipStuff(StuffItem item)
    {
        if (item == null) return;

        // Si un item est déjà équipé, le retirer
        if (m_equippedStuff.ContainsKey(item.Type))
        {
            var previousItem = m_equippedStuff[item.Type];
            // Optionnel : remettre dans l'inventaire si ce n'est pas déjà dedans
            if (!m_ownedStuff.Contains(previousItem))
            {
                m_ownedStuff.Add(previousItem);
            }
        }

        m_equippedStuff[item.Type] = item;
        m_ownedStuff.Remove(item); // Retire de l'inventaire non équipé
        OnStuffEquipped?.Invoke(item.Type, item);
    }

    public void UnequipStuff(StuffType type)
    {
        if (m_equippedStuff.TryGetValue(type, out StuffItem item))
        {
            m_equippedStuff.Remove(type);
            m_ownedStuff.Add(item); // Remet dans l'inventaire
            OnStuffEquipped?.Invoke(type, null);
        }
    }

    public StuffItem GetEquippedStuff(StuffType type)
    {
        return m_equippedStuff.ContainsKey(type) ? m_equippedStuff[type] : null;
    }

    public bool HasEquippedStuff(StuffType type)
    {
        return m_equippedStuff.ContainsKey(type);
    }

    public Dictionary<StuffType, StuffItem> GetAllEquippedStuff()
    {
        return new Dictionary<StuffType, StuffItem>(m_equippedStuff); // Retourne une copie
    }

    // ========== TOWER FRAGMENTS ==========
    public void AddTowerFragments(TowerType type, int amount)
    {
        if (amount < 0) return;

        if (!m_towerFragments.ContainsKey(type))
        {
            m_towerFragments[type] = 0;
        }
        m_towerFragments[type] += amount;
    }

    public int GetTowerFragments(TowerType type)
    {
        return m_towerFragments.TryGetValue(type, out int count) ? count : 0;
    }

    public bool SpendTowerFragments(TowerType type, int amount)
    {
        if (amount < 0) return false;

        if (m_towerFragments.TryGetValue(type, out int current) && current >= amount)
        {
            m_towerFragments[type] -= amount;
            return true;
        }

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

        if (!m_stuffBlueprints.ContainsKey(type))
        {
            m_stuffBlueprints[type] = 0;
        }
        m_stuffBlueprints[type] += amount;
    }

    public int GetStuffBlueprints(StuffType type)
    {
        return m_stuffBlueprints.TryGetValue(type, out int count) ? count : 0;
    }

    public bool SpendStuffBlueprints(StuffType type, int amount)
    {
        if (amount < 0) return false;

        if (m_stuffBlueprints.TryGetValue(type, out int current) && current >= amount)
        {
            m_stuffBlueprints[type] -= amount;
            return true;
        }

        return false;
    }

    public bool CanUpgradeStuff(StuffType type, int blueprintsRequired)
    {
        return GetStuffBlueprints(type) >= blueprintsRequired;
    }

    // ========== UTILITY ==========
    public void ResetInventory()
    {
        m_gold = 0;
        m_diamond = 0;
        m_ownedStuff.Clear();
        m_equippedStuff.Clear();
        m_towerFragments.Clear();
        m_stuffBlueprints.Clear();
        Debug.Log("Inventory reset");
    }

    // Debug : afficher tout l'inventaire
    public void LogInventory()
    {
        Debug.Log("===== INVENTORY =====");
        Debug.Log($"Gold: {m_gold}");
        Debug.Log($"Diamond: {m_diamond}");
        Debug.Log($"Owned Stuff: {m_ownedStuff.Count}");
        Debug.Log($"Equipped Stuff: {m_equippedStuff.Count}");
        Debug.Log($"Tower Fragments Types: {m_towerFragments.Count}");
        Debug.Log($"Stuff Blueprints Types: {m_stuffBlueprints.Count}");
    }
}
