using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerMenu : MonoBehaviour
{
    [SerializeField] GameObject m_towerPanelPrefab;
    [SerializeField] GridLayoutGroup m_group;

    List<TowerPanel> panels = new List<TowerPanel>();

    void Start()
    {
        foreach (TowerStats tower in DataManager.Instance.unlockedTowers) //need to use GameRegistry and add unlockValues on towerPanel
        {
            GameObject go = Instantiate(m_towerPanelPrefab, m_group.transform);
            TowerStats stats = go.GetComponent<TowerStats>();
            //set values
            stats = tower;
            panels.Add(go.GetComponent<TowerPanel>());
        }
    }

    //Projectile,
    //Railgun,
    //Laser,
    //Drone,
    //SlowArea,

    public void UnlockBasicTower()
    {

    }

    public void UnlockRailgunTower()
    {

    }

    public void UnlockLaserTower()
    {

    }

    public void UnlockDroneTower()
    {

    }
}
