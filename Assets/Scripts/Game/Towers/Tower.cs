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
    protected TowerStats m_stats;

    //RuntimeValues
    protected float m_damage;
    protected float m_cooldown;
    protected float m_range;
    protected float m_explosionDamage;
    protected float m_damageRange;
    protected float m_duration;
    protected float m_damageInterval;
    protected int m_nbAttack;
    protected int m_penetration;


    protected float m_currentCritRate;
    protected float m_currentCritMultiplier;

    void Start()
    {
        InitializeTower();
    }

    protected virtual void InitializeTower()
    {
        m_stats = DataManager.Instance.unlockedTowers.Find(t => t.type == m_type);

        m_damage = m_stats.damage;
        m_cooldown = m_stats.cooldown;
        m_range = m_stats.range;
        m_explosionDamage = m_stats.explosionDamage;
        m_damageRange = m_stats.damageRange;
        m_duration = m_stats.duration;
        m_damageInterval = m_stats.damageInterval;
        m_nbAttack = m_stats.nbAttack;
        m_penetration = m_stats.penetration;
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
