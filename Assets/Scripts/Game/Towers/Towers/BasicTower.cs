using System.Collections.Generic;
using UnityEngine;

public class BasicTower : Tower
{
    [Header("Base Stats")]
    [SerializeField] int m_baseNbProjectiles;

    //Privates Current values in run time
    int m_nbProjectiles;

    //Timer
    float m_timerShoot;

    private void Start()
    {
        InitializeTower();
    }

    protected override void InitializeTower()
    {
        base.InitializeTower();
        m_nbProjectiles = m_baseNbProjectiles;
    }

    void Update()
    {
        MoveLauncher();

        m_timerShoot += Time.deltaTime;

        if (m_timerShoot > (m_cooldown))
        {
            if (Shoot())
            {
                m_timerShoot = 0;
            }
        }
    }

    bool Shoot()
    {
        if (EnemyManager.Instance.Enemies.Count > 0)
        {
            Enemy target = EnemyManager.Instance.GetNearestEnemy(transform);

            float dist = Vector2.Distance(m_barrel.position, target.transform.position);

            if (dist <= m_range)
            {
                Vector2 direction;
                Vector2 targetPos = target.transform.position;
                if (m_nbProjectiles % 2 == 0)
                {
                    for (int i = 0; i < m_nbProjectiles; i++)
                    {
                        float targetDist;
                        if (i < m_nbProjectiles / 2)
                        {
                            targetDist = (-1f) * 0.5f * i;
                        }
                        else
                        {
                            targetDist = 0.5f * (i - m_nbProjectiles / 2);
                        }

                        Vector3 posTarget = new Vector3(targetPos.x + targetDist, targetPos.y);
                        direction = posTarget - m_barrel.position;
                        direction.Normalize();

                        GameObject go = Instantiate(m_projectilesPrefab, m_barrel.position, Quaternion.identity);
                        go.GetComponent<PlayerProjectiles>().Direction = direction;
                        go.GetComponent<PlayerProjectiles>().SetStats(m_damage, m_penetration);
                    }
                }
                else
                {
                    for (int i = 0; i < m_nbProjectiles; i++)
                    {
                        float targetDist = 0;
                        if (i < m_nbProjectiles / 2)
                        {
                            targetDist = (-1f) * 0.5f * i;
                        }
                        else if (i > m_nbProjectiles / 2)
                        {
                            targetDist = 0.5f * (i - m_nbProjectiles / 2);
                        }

                        Vector3 posTarget = new Vector3(targetPos.x + targetDist, targetPos.y);
                        direction = posTarget - m_barrel.position;
                        direction.Normalize();

                        GameObject go = Instantiate(m_projectilesPrefab, m_barrel.position, Quaternion.identity);
                        go.GetComponent<PlayerProjectiles>().Direction = direction;
                        go.GetComponent<PlayerProjectiles>().SetStats(m_damage, m_penetration);
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

            case "IncreaseNbProjectiles":
                IncreaseNbProjectiles();
                break;
        }
    }

    private void IncreaseDamage()
    {
        m_damage += m_stats.damage * 0.5f;
    }

    private void IncreaseNbProjectiles()
    {
        m_nbProjectiles += 1;
    }
}
