using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentInventory : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GridLayoutGroup m_grid;
    [SerializeField] GameObject m_equipmentPrefab;

    void Start()
    {
        foreach (StuffItem stuff in InventoryManager.Instance.m_ownedStuff)
        {
            GameObject go = Instantiate(m_equipmentPrefab, m_grid.transform);
            go.GetComponent<EquipmentUI>().stuff = stuff;
        }
    }
}
