using UnityEngine;
using UnityEngine.UI;

public class TowerMenu : MonoBehaviour
{
    [SerializeField] GameObject m_towerPanelPrefab;
    [SerializeField] GridLayoutGroup m_group;

    void Start()
    {
        foreach (TowerStats tower in DataManager.Instance.unlockedTowers)
        {
            GameObject go = Instantiate(m_towerPanelPrefab, m_group.transform);
            TowerStats stats = go.GetComponent<TowerStats>();
            //set values
            stats = tower;
        }
    }

    void Update()
    {
        
    }
}
