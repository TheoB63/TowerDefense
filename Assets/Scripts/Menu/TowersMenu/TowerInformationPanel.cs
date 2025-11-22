using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerInformationPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_towerName;
    [SerializeField] Image m_icon;
    [SerializeField] TextMeshProUGUI m_description;
    [SerializeField] GridLayoutGroup m_group;

    [SerializeField] GameObject m_statPanelPrefab;

    public TowerType type;

    List<GameObject> m_statsPanels = new List<GameObject>();

    public void Setup(TowerType _type)
    {
        type = _type;
        TowerData data = GameRegistry.Instance.GetTowerByType(type);
        TowerStats stats = DataManager.Instance.unlockedTowers.Find(t => t.type == type);

        m_towerName.text = data.towerName + " Niv." + stats.level;
        m_icon.sprite = data.sprite;

        foreach (GameObject panel in m_statsPanels)
        {
            Destroy(panel);
        }
        m_statsPanels.Clear();

        GameObject typePanel = Instantiate(m_statPanelPrefab, m_group.transform);
        typePanel.GetComponent<TowerStatPanel>().Setup("Type", data.damageType.ToString());
        m_statsPanels.Add(typePanel);

        GameObject damagePanel = Instantiate(m_statPanelPrefab, m_group.transform);
        damagePanel.GetComponent<TowerStatPanel>().Setup("Damage", Mathf.Floor(stats.damage).ToString());
        m_statsPanels.Add(damagePanel);

        if (stats.explosionDamage > 0)
        {
            GameObject explosionDamagePanel = Instantiate(m_statPanelPrefab, m_group.transform);
            explosionDamagePanel.GetComponent<TowerStatPanel>().Setup("Explosion Damage", Mathf.Floor(stats.explosionDamage).ToString());
            m_statsPanels.Add(explosionDamagePanel);
        }
        if (stats.damageRange > 0)
        {
            GameObject damageRangeDamagePanel = Instantiate(m_statPanelPrefab, m_group.transform);
            damageRangeDamagePanel.GetComponent<TowerStatPanel>().Setup("Damage Range", Mathf.Floor(stats.damageRange).ToString());
            m_statsPanels.Add(damageRangeDamagePanel);
        }
        if (stats.duration > 0)
        {
            GameObject durationPanel = Instantiate(m_statPanelPrefab, m_group.transform);
            durationPanel.GetComponent<TowerStatPanel>().Setup("Duration", stats.duration.ToString());
            m_statsPanels.Add(durationPanel);
        }
        if (stats.damageInterval > 0)
        {
            GameObject damageIntervalPanel = Instantiate(m_statPanelPrefab, m_group.transform);
            damageIntervalPanel.GetComponent<TowerStatPanel>().Setup("Damage IntervalPanel", stats.damageInterval.ToString());
            m_statsPanels.Add(damageIntervalPanel);
        }
        if (stats.nbAttack > 0)
        {
            GameObject nbAttackPanel = Instantiate(m_statPanelPrefab, m_group.transform);
            nbAttackPanel.GetComponent<TowerStatPanel>().Setup("Nb Attack", stats.nbAttack.ToString());
            m_statsPanels.Add(nbAttackPanel);
        }
        if (stats.penetration > 0)
        {
            GameObject penetrationPanel = Instantiate(m_statPanelPrefab, m_group.transform);
            penetrationPanel.GetComponent<TowerStatPanel>().Setup("Penetration", stats.penetration.ToString());
            m_statsPanels.Add(penetrationPanel);
        }

        GameObject coolDownPanel = Instantiate(m_statPanelPrefab, m_group.transform);
        coolDownPanel.GetComponent<TowerStatPanel>().Setup("Cooldown", stats.cooldown.ToString());
        m_statsPanels.Add(coolDownPanel);

        GameObject rangePanel = Instantiate(m_statPanelPrefab, m_group.transform);
        rangePanel.GetComponent<TowerStatPanel>().Setup("Range", stats.range.ToString());
        m_statsPanels.Add(rangePanel);
    }
}