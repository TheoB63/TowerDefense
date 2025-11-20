using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TowerMenu : MonoBehaviour
{
    [SerializeField] GameObject m_towerPanelPrefab;
    [SerializeField] GridLayoutGroup m_group;
    [SerializeField] GameObject m_informationPanel;

    List<TowerPanel> panels = new List<TowerPanel>();

    private void Start()
    {
        foreach (TowerData tower in GameRegistry.Instance.towers)
        {
            GameObject go = Instantiate(m_towerPanelPrefab, m_group.transform);
            go.GetComponent<TowerPanel>().type = tower.type;
            go.GetComponent<TowerPanel>().towerMenu = this;

            if (DataManager.Instance.unlockedTowers.Any(t=>t.type == tower.type))
            {
                go.GetComponent<TowerPanel>().unlock = true;
            }

            panels.Add(go.GetComponent<TowerPanel>());
        }
    }


    public void OpenTowerInformationPanel(TowerType _type)
    {
        m_informationPanel.SetActive(true);
        m_informationPanel.GetComponent<TowerInformationPanel>().Setup(_type);
    }

    public void CloseTowerInformationPanel()
    {
        m_informationPanel.SetActive(false);
    }

    public void UpgradeSelectedTower()
    {
        TowerType selectedType = m_informationPanel.GetComponent<TowerInformationPanel>().type;
        TowerStats stats = DataManager.Instance.unlockedTowers.Find(t => t.type == selectedType);

        //Upgrade Stats
        stats.level++;

        //Update Panels
        TowerPanel panel = panels.Find(t => t.type == selectedType);
        panel.UpdatePanel();
        m_informationPanel.GetComponent<TowerInformationPanel>().Setup(selectedType);

        Debug.Log("fake upgrade " + selectedType);
    }
}