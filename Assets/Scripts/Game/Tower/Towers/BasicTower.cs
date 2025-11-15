using System.Collections.Generic;
using UnityEngine;

public class BasicTower : Tower
{
    [Header("Base Stats")]
    [SerializeField] float m_baseAttackSpeed;
    [SerializeField] int m_baseNbProjectiles;
    [SerializeField] int m_basePenetration;

    [Header("Upgrades")]
    [SerializeField] float m_attackSpeedMultiplier;
    [SerializeField] int m_nbProjectilesIncrease;
    [SerializeField] int m_penetrationIncrease;

    //Privates Current values in run time
    float m_currentAttackSpeed;
    int m_currentNbProjectiles;
    int m_currentPenetration;

    [Header("Public Values (Get Only)")]
    public float AttackSpeed { get => m_currentAttackSpeed; set => m_currentAttackSpeed = value; }
    public int NbProjectiles { get => m_currentNbProjectiles; set => m_currentNbProjectiles = value; }
    public int Penetration { get => m_currentPenetration; set => m_currentPenetration = value; }

    //Timer
    float m_timerShoot;

    private void Start()
    {
        InitializeTower();
    }

    protected override void InitializeTower()
    {
        base.InitializeTower();
        m_currentAttackSpeed = m_baseAttackSpeed;
        m_currentNbProjectiles = m_baseNbProjectiles;
        m_currentPenetration = m_basePenetration;
    }

    void Update()
    {
        MoveLauncher();

        m_timerShoot += Time.deltaTime;

        if (m_timerShoot > (1 / m_currentAttackSpeed))
        {
            if (Shoot())
            {
                m_timerShoot = 0;
            }
        }
    }

    bool Shoot()
    {
        if (GameManager.Instance.Enemies.Count > 0)
        {
            Enemy target = GameManager.Instance.GetNearestEnemy(transform);

            float dist = Vector2.Distance(m_barrel.position, target.transform.position);

            if (dist <= m_currentRange)
            {
                Vector2 direction;
                Vector2 targetPos = target.transform.position;
                if (m_currentNbProjectiles % 2 == 0)
                {
                    for (int i = 0; i < m_currentNbProjectiles; i++)
                    {
                        float targetDist;
                        if (i < m_currentNbProjectiles / 2)
                        {
                            targetDist = (-1f) * 0.5f * i;
                        }
                        else
                        {
                            targetDist = 0.5f * (i - m_currentNbProjectiles / 2);
                        }

                        Vector3 posTarget = new Vector3(targetPos.x + targetDist, targetPos.y);
                        direction = posTarget - m_barrel.position;
                        direction.Normalize();

                        GameObject go = Instantiate(m_projectilesPrefab, m_barrel.position, Quaternion.identity);
                        go.GetComponent<PlayerProjectiles>().Direction = direction;
                        go.GetComponent<PlayerProjectiles>().SetStats(m_currentDamage, m_currentPenetration);
                    }
                }
                else
                {
                    for (int i = 0; i < m_currentNbProjectiles; i++)
                    {
                        float targetDist = 0;
                        if (i < m_currentNbProjectiles / 2)
                        {
                            targetDist = (-1f) * 0.5f * i;
                        }
                        else if (i > m_currentNbProjectiles / 2)
                        {
                            targetDist = 0.5f * (i - m_currentNbProjectiles / 2);
                        }

                        Vector3 posTarget = new Vector3(targetPos.x + targetDist, targetPos.y);
                        direction = posTarget - m_barrel.position;
                        direction.Normalize();

                        GameObject go = Instantiate(m_projectilesPrefab, m_barrel.position, Quaternion.identity);
                        go.GetComponent<PlayerProjectiles>().Direction = direction;
                        go.GetComponent<PlayerProjectiles>().SetStats(m_currentDamage, m_currentPenetration);
                    }
                }
                return true;
            }
        }
        return false;
    }

    public override void ApplyUpgrade(string _upgradeName)
    {
        switch (_upgradeName)
        {
            case "IncreaseDamage":
                IncreaseDamage();
                break;

            case "IncreaseRange":
                IncreaseRange();
                break;

            case "IncreaseAttackSpeed":
                IncreaseAttackSpeed();
                break;

            case "IncreasePenetration":
                IncreasePenetration();
                break;

            case "IncreaseNbProjectiles":
                IncreaseNbProjectiles();
                break;
        }
    }

    public override List<UpgradeBase> GetAvailableUpgrades()
    {
        List<UpgradeBase> available = new List<UpgradeBase>();

        foreach (TowerUpgrade upgrade in m_towerUpgrades)
        {
            TowerUpgrade copy = Instantiate(upgrade);
            available.Add(copy);
        }
        return available;
    }

    private void IncreaseDamage()
    {
        m_currentDamage = (int)(m_currentDamage * m_damageMultiplier);
    }
    private void IncreaseRange()
    {
        m_currentRange += m_rangeIncrease;
    }
    private void IncreaseAttackSpeed()
    {
        m_currentAttackSpeed *= m_attackSpeedMultiplier;
    }

    private void IncreasePenetration()
    {
        m_currentPenetration += m_penetrationIncrease;
    }

    private void IncreaseNbProjectiles()
    {
        m_currentNbProjectiles += m_nbProjectilesIncrease;
    }
}
