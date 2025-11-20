using TMPro;
using UnityEngine;

public class TowerInformationPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_towerName;

    public TowerType type;

    public void Setup(TowerType _type)
    {
        type = _type;
        TowerData data = GameRegistry.Instance.GetTowerByType(type);
        TowerStats stats = DataManager.Instance.unlockedTowers.Find(t => t.type == type);

        m_towerName.text = data.towerName + " Niv." + stats.level;
    }
}
