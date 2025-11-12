using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //instance
    static public LevelManager Instance;

    [Header("References")]
    [SerializeField] Player m_player;
    [SerializeField] List<TowerSlot> m_towers;
    [SerializeField] TowerManager m_towerManager;

    //level
    int m_level = 0;
    float m_xp = 0;
    [SerializeField] float m_xpNeeded;
    [SerializeField] float m_xpNeededMultiplier;


    [Header("Public Values(Get only)")]
    public int Level { get => m_level; }
    public float Xp { get => m_xp; }
    public float XpNeeded { get => m_xpNeeded; }


    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {

    }

    public void AddXp(float _xp)
    {
        m_xp += _xp;
        LevelUp();
        UI.Instance.UpdateUI();
    }

    void LevelUp()
    {
        if (m_xp >= m_xpNeeded)
        {
            m_level++;
            m_xp -= m_xpNeeded;
            m_xpNeeded *= m_xpNeededMultiplier;
            SetChoseUpgrade();
        }
    }

    void SetChoseUpgrade()
    {
        List<UpgradeTower> options = m_towerManager.GetAvailableChoices();


        Debug.Log("options :" + options.Count);
        UI.Instance.ShowUpgradeChoices(options);
    }
}
