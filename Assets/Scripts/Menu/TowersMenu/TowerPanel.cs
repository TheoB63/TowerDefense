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
    [SerializeField] Image m_lockImage;

    TowerStats m_towerStats;
    TowerData m_towerData;
    public bool Unlock = false;

    private void Awake()
    {
        m_towerStats = GetComponent<TowerStats>();
        m_towerData = GameRegistry.Instance.GetTowerByType(m_towerStats.type);
        m_nameText.text = m_towerData.towerName;
        m_icon.sprite = m_towerData.sprite;

        UpdatePanel();
    }

    void UpdatePanel()
    {
        m_lvlText.text = "Lvl." + m_towerStats.level.ToString();

        m_costBar.fillAmount = 0.1f;
        m_costText.text = "10/100";

        Color color = m_lockImage.color;
        float alpha = Unlock ? 0f : 0.9f;
        m_lockImage.color = new Color(color.r, color.g, color.b, alpha);
    }

    public void UnlockTower()
    {
        Unlock = true;
        UpdatePanel();
    }
}
