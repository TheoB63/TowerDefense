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
                float spacing = 0.5f;
                int nb = m_nbProjectiles;

                float startOffset = -((nb - 1) * spacing) / 2f;

                for (int i = 0; i < nb; i++)
                {
                    float targetDist = startOffset + (i * spacing);

                    Vector3 posTarget = new Vector3(targetPos.x + targetDist, targetPos.y);
                    direction = (Vector2)posTarget - (Vector2)transform.position;
                    direction.Normalize();

                    GameObject go = Instantiate(m_projectilesPrefab, m_barrel.position, Quaternion.identity);
                    go.GetComponent<PlayerProjectiles>().Direction = direction;
                    go.GetComponent<PlayerProjectiles>().SetStats(m_damage, m_penetration);
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
