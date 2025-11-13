using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] GameObject m_projectilesPrefab;

    //Timer
    float m_timerShoot;
    PlayerStats m_playerStats;

    void Start()
    {
        m_playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        m_timerShoot += Time.deltaTime;

        if (m_timerShoot > (1 / m_playerStats.AttackSpeed))
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
            Enemy target = GetNearestEnemy();

            float dist = Vector2.Distance(transform.position, target.transform.position);

            if (dist <= GetComponent<PlayerStats>().Range)
            {
                Vector2 direction;
                Vector2 targetPos = target.transform.position;
                if (m_playerStats.NbProjectiles % 2 == 0)
                {
                    for (int i = 0; i < m_playerStats.NbProjectiles; i++)
                    {
                        float targetDist;
                        if (i < m_playerStats.NbProjectiles / 2)
                        {
                            targetDist = (-1f) * 0.5f * i;
                        }
                        else
                        {
                            targetDist = 0.5f * (i - m_playerStats.NbProjectiles / 2);
                        }

                        Vector3 posTarget = new Vector3(targetPos.x + targetDist, targetPos.y);
                        direction = posTarget - transform.position;
                        direction.Normalize();

                        GameObject go = Instantiate(m_projectilesPrefab, transform.position, Quaternion.identity);
                        go.GetComponent<Projectiles>().Direction = direction;
                        go.GetComponent<Projectiles>().SetStats(m_playerStats.Damage, m_playerStats.Penetration);
                    }
                }
                else
                {
                    for (int i = 0; i < m_playerStats.NbProjectiles; i++)
                    {
                        float targetDist = 0;
                        if (i < m_playerStats.NbProjectiles / 2)
                        {
                            targetDist = (-1f) * 0.5f * i;
                        }
                        else if (i > m_playerStats.NbProjectiles / 2)
                        {
                            targetDist = 0.5f * (i - m_playerStats.NbProjectiles / 2);
                        }

                        Vector3 posTarget = new Vector3(targetPos.x + targetDist, targetPos.y);
                        direction = posTarget - transform.position;
                        direction.Normalize();

                        GameObject go = Instantiate(m_projectilesPrefab, transform.position, Quaternion.identity);
                        go.GetComponent<Projectiles>().Direction = direction;
                        go.GetComponent<Projectiles>().SetStats(m_playerStats.Damage, m_playerStats.Penetration);
                    }
                }
                return true;
            }
        }
        return false;
    }

    private Enemy GetNearestEnemy()
    {
        Enemy target = GameManager.Instance.Enemies[0];

        float dist = float.MaxValue;
        foreach (Enemy enemy in GameManager.Instance.Enemies)
        {
            float newDist = Vector2.Distance(transform.position, enemy.transform.position);
            if (newDist < dist)
            {
                target = enemy;
                dist = newDist;
            }
        }

        return target;
    }
}
