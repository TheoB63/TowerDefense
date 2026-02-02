using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class EquipmentInventory : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GridLayoutGroup m_grid;
    [SerializeField] GameObject m_equipmentPrefab;

    List<EquipmentUI> m_equipments;

    void Start()
    {
        foreach (EquipmentItem stuff in InventoryManager.Instance.m_ownedEquipment)
        {
            GameObject go = Instantiate(m_equipmentPrefab, m_grid.transform);
            go.GetComponent<EquipmentUI>().stuff = stuff;
        }
    }

    public void UpdateUI()
    {
        List<EquipmentItem> equipments = InventoryManager.Instance.m_ownedEquipment;

        ////delete
        //foreach (EquipmentUI equipmentUI in m_equipments)
        //{
        //    StuffItem equipment = equipments.Find(equipment => equipment == equipmentUI);
        //    if (item == null)
        //    {
        //        Destroy(slot.gameObject);
        //        m_slots.Remove(slot);
        //    }
        //}

        ////create
        //foreach (StuffItem stuff in equipments)
        //{
        //    ItemSlot slot = m_slots.Find(slot => slot.itemRef == item);
        //    if (slot)
        //    {
        //        slot.UpdateSlot();
        //    }
        //    else
        //    {
        //        GameObject go = Instantiate(m_slotPrefab, m_group.transform, false);
        //        slot = go.GetComponent<ItemSlot>();
        //        slot.itemRef = item;
        //        slot.UpdateSlot();
        //        m_slots.Add(slot);
        //    }
        //}
    }
}
