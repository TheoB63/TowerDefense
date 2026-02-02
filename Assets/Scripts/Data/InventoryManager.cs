using System;
using System.Collections.Generic;
using UnityEngine;

public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

#region Equipment
public enum EquipmentType
{
    Helmet,
    Plate,
    Legs,
    Boots,
    Shield,
    Canon
}

[System.Serializable]
public class EquipmentItem
{
    public EquipmentType Type;
    public Rarity Rarity;
    public int Level;
    public int Attack;
}
#endregion


public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    //Basic
    public int m_gold;
    public int m_diamond;

    //Stuff
    public List<EquipmentItem> m_ownedEquipment = new List<EquipmentItem>();
    public Dictionary<EquipmentType, EquipmentItem> m_equippedEquipment = new Dictionary<EquipmentType, EquipmentItem>();

    //Fragments upgrade
    public Dictionary<TowerType, int> m_towerFragments = new Dictionary<TowerType, int>();
    public Dictionary<EquipmentType, int> m_equipmentBlueprints = new Dictionary<EquipmentType, int>();

    //Upgrades


    //Events
    public event Action<int> OnGoldChanged;
    public event Action<int> OnDiamondChanged;
    public event Action<EquipmentType, EquipmentItem> OnEquipmentEquipped;

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
    public void AddStuff(EquipmentItem item)
    {
        if (item == null) return;
        m_ownedEquipment.Add(item);
    }

    public void RemoveStuff(EquipmentItem item)
    {
        m_ownedEquipment.Remove(item);
    }

    public List<EquipmentItem> GetOwnedStuff()
    {
        return new List<EquipmentItem>(m_ownedEquipment); // Retourne une copie
    }

    public List<EquipmentItem> GetStuffByType(EquipmentType type)
    {
        return m_ownedEquipment.FindAll(s => s.Type == type);
    }

    // ========== EQUIPPED STUFF ==========
    public void EquipStuff(EquipmentItem item)
    {
        if (item == null) return;

        // Si un item est déjà équipé, le retirer
        if (m_equippedEquipment.ContainsKey(item.Type))
        {
            var previousItem = m_equippedEquipment[item.Type];
            // Optionnel : remettre dans l'inventaire si ce n'est pas déjà dedans
            if (!m_ownedEquipment.Contains(previousItem))
            {
                m_ownedEquipment.Add(previousItem);
            }
        }

        m_equippedEquipment[item.Type] = item;
        m_ownedEquipment.Remove(item); // Retire de l'inventaire non équipé
        OnEquipmentEquipped?.Invoke(item.Type, item);
    }

    public void UnequipStuff(EquipmentType type)
    {
        if (m_equippedEquipment.TryGetValue(type, out EquipmentItem item))
        {
            m_equippedEquipment.Remove(type);
            m_ownedEquipment.Add(item); // Remet dans l'inventaire
            OnEquipmentEquipped?.Invoke(type, null);
        }
    }

    public EquipmentItem GetEquippedStuff(EquipmentType type)
    {
        return m_equippedEquipment.ContainsKey(type) ? m_equippedEquipment[type] : null;
    }

    public bool HasEquippedStuff(EquipmentType type)
    {
        return m_equippedEquipment.ContainsKey(type);
    }

    public Dictionary<EquipmentType, EquipmentItem> GetAllEquippedStuff()
    {
        return new Dictionary<EquipmentType, EquipmentItem>(m_equippedEquipment); // Retourne une copie
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
    public void AddStuffBlueprints(EquipmentType type, int amount)
    {
        if (amount < 0) return;

        if (!m_equipmentBlueprints.ContainsKey(type))
        {
            m_equipmentBlueprints[type] = 0;
        }
        m_equipmentBlueprints[type] += amount;
    }

    public int GetStuffBlueprints(EquipmentType type)
    {
        return m_equipmentBlueprints.TryGetValue(type, out int count) ? count : 0;
    }

    public bool SpendStuffBlueprints(EquipmentType type, int amount)
    {
        if (amount < 0) return false;

        if (m_equipmentBlueprints.TryGetValue(type, out int current) && current >= amount)
        {
            m_equipmentBlueprints[type] -= amount;
            return true;
        }

        return false;
    }

    public bool CanUpgradeStuff(EquipmentType type, int blueprintsRequired)
    {
        return GetStuffBlueprints(type) >= blueprintsRequired;
    }

    // ========== UTILITY ==========
    public void ResetInventory()
    {
        m_gold = 0;
        m_diamond = 0;
        m_ownedEquipment.Clear();
        m_equippedEquipment.Clear();
        m_towerFragments.Clear();
        m_equipmentBlueprints.Clear();
        Debug.Log("Inventory reset");
    }

    //Draw Debug
    public void LogInventory()
    {
        Debug.Log("===== INVENTORY =====");
        Debug.Log($"Gold: {m_gold}");
        Debug.Log($"Diamond: {m_diamond}");
        Debug.Log($"Owned Stuff: {m_ownedEquipment.Count}");
        Debug.Log($"Equipped Stuff: {m_equippedEquipment.Count}");
        Debug.Log($"Tower Fragments Types: {m_towerFragments.Count}");
        Debug.Log($"Stuff Blueprints Types: {m_equipmentBlueprints.Count}");
    }
}
