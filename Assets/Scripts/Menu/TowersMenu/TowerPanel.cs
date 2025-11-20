using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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

    public TowerType type;
    public bool unlock = false;

    private void Start()
    {
        TowerData data = GameRegistry.Instance.GetTowerByType(type);
        m_nameText.text = data.towerName;
        m_icon.sprite = data.sprite;

        UpdatePanel();
    }

    void UpdatePanel()
    {
        if (DataManager.Instance.unlockedTowers.Any(t => t.type == type))
        {
            TowerStats stats = DataManager.Instance.unlockedTowers.Find(t => t.type == type);
            m_lvlText.text = "Lvl." + stats.level.ToString();
        }
        else
        {
            m_lvlText.text = "Lvl.0";
        }

        m_costBar.fillAmount = 0.1f;
        m_costText.text = "10/100";

        Color color = m_lockImage.color;
        float alpha = unlock ? 0f : 0.9f;
        m_lockImage.color = new Color(color.r, color.g, color.b, alpha);
    }

    public void UnlockTower()
    {
        unlock = true;
        UpdatePanel();
    }
}
