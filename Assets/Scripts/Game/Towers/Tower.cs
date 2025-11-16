using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour, ITower
{
    [Header("References")]
    [SerializeField] protected GameObject m_projectilesPrefab;
    [SerializeField] protected Transform m_barrel;
    [SerializeField] protected Transform m_launcher;
    [SerializeField] protected float m_armRotationSpeed;

    [Header("Bases Stats")]
    [SerializeField] protected TowerType m_type;
    [SerializeField] protected string m_towerName;

    [SerializeField] protected int m_baseDamage;
    [SerializeField] protected float m_baseRange;
    [SerializeField] protected float m_baseCritRate;
    [SerializeField] protected float m_baseCritMultiplier;

    [Header("Upgrades")]
    [SerializeField] protected List<TowerUpgrade> m_towerUpgrades;
    [SerializeField] protected float m_damageMultiplier;
    [SerializeField] protected int m_rangeIncrease;
    [SerializeField] protected float m_critRateIncrease;
    [SerializeField] protected float m_CritMultiplierIncrease;

    //RuntimeValues
    protected int m_currentDamage;
    protected float m_currentRange;
    protected float m_currentCritRate;
    protected float m_currentCritMultiplier;

    //Public Values
    public int Damage { get => m_currentDamage; set => m_currentDamage = value; }
    public float Range { get => m_currentRange; set => m_currentRange = value; }
    public float CritRate { get => m_currentCritRate; set => m_currentCritRate = value; }
    public float CritMultiplier { get => m_currentCritMultiplier; set => m_currentCritMultiplier = value; }


    void Start()
    {
        InitializeTower();
    }

    protected virtual void InitializeTower()
    {
        m_currentDamage = m_baseDamage;
        m_currentRange = m_baseRange;
        m_currentCritRate = m_baseCritRate;
        m_currentCritMultiplier = m_baseCritMultiplier;
    }

    protected virtual void MoveLauncher()
    {
        if (EnemyManager.Instance.Enemies.Count > 0)
        {
            Enemy target = EnemyManager.Instance.GetNearestEnemy(transform);
            Vector2 dir = target.transform.position - m_launcher.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion targetRot = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
            m_launcher.rotation = Quaternion.Lerp(m_launcher.rotation, targetRot, m_armRotationSpeed * Time.deltaTime);
        }
    }

    public virtual TowerType GetTowerType() => m_type;
    public virtual string GetTowerName() => m_towerName;
    public virtual void ApplyUpgrade(string _upgradeName) { }
    public virtual List<UpgradeBase> GetAvailableUpgrades() => new List<UpgradeBase>();
}
