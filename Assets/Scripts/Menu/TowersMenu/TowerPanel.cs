using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_nameText;
    [SerializeField] Image m_icon;
    [SerializeField] TextMeshProUGUI m_lvlText;
    [SerializeField] Image m_costBar;
    [SerializeField] TextMeshProUGUI m_costText;

    TowerStats m_towerStats;
    TowerData m_towerData;

    void Start()
    {
        m_towerStats = GetComponent<TowerStats>();
        m_towerData = GameRegistry.Instance.GetTowerByType(m_towerStats.type);
        m_nameText.text = m_towerData.towerName;
        m_icon.sprite = m_towerData.sprite;
        m_lvlText.text = "Niv." + m_towerStats.level.ToString();

        m_costBar.fillAmount = 0.1f;
        m_costText.text = "10/100";
    }

    void Update()
    {

    }
}
