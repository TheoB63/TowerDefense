using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] GameObject m_projectilesPrefab;
    [SerializeField] float m_attackSpeed;

    //Timer
    float m_timerShoot;

    void Start()
    {
    }

    void Update()
    {
        m_timerShoot += Time.deltaTime;

        if (m_timerShoot > (m_attackSpeed*GetComponent<PlayerStats>().globalAttackSpeedBonus / 60))
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
            Enemy currentTarget = GameManager.Instance.Enemies[0];
            float dist = float.MaxValue;
            foreach (Enemy enemy in GameManager.Instance.Enemies)
            {
                float newDist = Vector2.Distance(transform.position, enemy.transform.position);
                if (newDist < dist)
                {
                    currentTarget = enemy;
                    dist = newDist;
                }
            }
            if (dist <= GetComponent<PlayerStats>().currentRange)
            {
                GameObject go = Instantiate(m_projectilesPrefab, transform.position, Quaternion.identity);
                go.GetComponent<Projectiles>().Target = currentTarget;
                go.GetComponent<Projectiles>().Damage = GetComponent<PlayerStats>().damage;

                return true;
            }
        }
        return false;
    }
}
