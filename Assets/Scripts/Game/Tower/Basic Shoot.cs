using UnityEngine;

public class BasicShoot : MonoBehaviour
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

        if (m_timerShoot > (m_attackSpeed / 60))
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
                }
            }
            GameObject go = Instantiate(m_projectilesPrefab, transform.position, Quaternion.identity);
            go.GetComponent<Projectiles>().Target = currentTarget;

            return true;
        }
        return false;
    }
}
