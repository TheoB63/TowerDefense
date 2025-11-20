using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerMenu : MonoBehaviour
{
    [SerializeField] GameObject m_towerPanelPrefab;
    [SerializeField] GridLayoutGroup m_group;

    List<TowerPanel> panels = new List<TowerPanel>();

    private void Awake() //awake because if it's new game need to be load before
    {
        //Debug.Log("awake tower");

        //foreach (TowerData tower in GameRegistry.Instance.towers)
        //{
        //    GameObject go = Instantiate(m_towerPanelPrefab, m_group.transform);
        //    //TowerStats stats = go.GetComponent<TowerStats>();

        //    TowerStats stats = DataManager.Instance.unlockedTowers.Find(t => t.type == tower.type);

        //    if ()
        //    {
        //        Debug.Log("Tower find in mana : " + tower.type);
        //        //stats = DataManager.Instance.unlockedTowers.Find(t => t.GetComponent<TowerStats>().type == tower.type);
        //    }
        //    else
        //    {
        //        Debug.Log("setType to panel");
        //        //stats.type = tower.type;
        //    }
        //    panels.Add(go.GetComponent<TowerPanel>());
        //}
    }

    public void UnlockBasicTower()
    {
        Debug.Log("unlock basic");

        TowerPanel panel = panels.Find(p => p.GetComponent<TowerStats>().type == TowerType.Basic);
        if (panel != null)
        {
            panel.UnlockTower();
            DataManager.Instance.unlockedTowers.Add(panel.GetComponent<TowerStats>());
        }
        else
        {
            Debug.Log("panel null");
        }
    }


    public void UnlockRailgunTower()
    {
        TowerPanel panel = panels.Find(p => p.GetComponent<TowerStats>().type == TowerType.Railgun);
        panel.UnlockTower();
        DataManager.Instance.unlockedTowers.Add(panel.GetComponent<TowerStats>());

    }

    public void UnlockLaserTower()
    {

    }

    public void UnlockDroneTower()
    {

    }
}
