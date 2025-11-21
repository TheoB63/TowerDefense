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
        TowerData data = GameRegistry.Instance.GetTowerByType(selectedType);

        //Upgrade Stats
        stats.level++;
        stats.damage *= data.damageUpgrades;
        stats.cooldown -= data.GetUpgradeStatForLevel(StatType.Cooldown, stats.level);
        stats.range += data.GetUpgradeStatForLevel(StatType.Range, stats.level);
        stats.explosionDamage += data.GetUpgradeStatForLevel(StatType.ExplosionDamage, stats.level);
        stats.damageRange += data.GetUpgradeStatForLevel(StatType.DamageRange, stats.level);
        stats.duration += data.GetUpgradeStatForLevel(StatType.Duration, stats.level);
        stats.damageInterval += data.GetUpgradeStatForLevel(StatType.DamageInterval, stats.level);
        stats.nbAttack += data.GetUpgradeStatForLevel(StatType.NbAttack, stats.level);
        stats.penetration += data.GetUpgradeStatForLevel(StatType.Penetration, stats.level);

        //Update Panels
        TowerPanel panel = panels.Find(t => t.type == selectedType);
        panel.UpdatePanel();
        m_informationPanel.GetComponent<TowerInformationPanel>().Setup(selectedType);

        Debug.Log("fake upgrade " + selectedType);
    }
}