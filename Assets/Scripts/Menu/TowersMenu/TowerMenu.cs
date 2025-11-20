using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TowerMenu : MonoBehaviour
{
    [SerializeField] GameObject m_towerPanelPrefab;
    [SerializeField] GridLayoutGroup m_group;

    List<TowerPanel> panels = new List<TowerPanel>();

    private void Start()
    {
        foreach (TowerData tower in GameRegistry.Instance.towers)
        {
            GameObject go = Instantiate(m_towerPanelPrefab, m_group.transform);
            go.GetComponent<TowerPanel>().type = tower.type;

            if (DataManager.Instance.unlockedTowers.Any(t=>t.type == tower.type))
            {
                go.GetComponent<TowerPanel>().unlock = true;
            }

            panels.Add(go.GetComponent<TowerPanel>());
        }
    }
}
