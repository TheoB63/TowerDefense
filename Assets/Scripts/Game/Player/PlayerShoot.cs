using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Refenrences")]
    [SerializeField] GameObject m_bulletPrefab;
    [SerializeField] GameObject m_subBulletPrefab;
    [SerializeField] List<Transform> m_arms;
    [SerializeField] List<Transform> m_fireCanons;

    [Header("Values")]
    [SerializeField] float m_armRotationSpeed;
    
    PlayerStats m_playerStats;
    int m_currentFireCanon = 0;

    //Timer
    float m_timerShoot;

    void Start()
    {
        m_playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        MoveFireCanon();

        m_timerShoot += Time.deltaTime;

        if (m_timerShoot > (1 / m_playerStats.GetFireRate))
        {
            if (Shoot())
            {
                m_timerShoot = 0;
            }
        }
    }

    bool Shoot()
    {
        Transform fireCanon = GetFireCanon();

        if (EnemyManager.Instance.Enemies.Count > 0)
        {
            Enemy target = EnemyManager.Instance.GetNearestEnemy(transform);

            float dist = Vector2.Distance(transform.position, target.transform.position);

            if (dist <= GetComponent<PlayerStats>().GetRange)
            {
                //Bullets
                Vector2 direction;
                Vector2 targetPos = target.transform.position;

                float spacingBullets = 0.5f;
                int nb = m_playerStats.GetNbBullets;
                float startOffset = -((nb - 1) * spacingBullets) / 2f;

                for (int i = 0; i < nb; i++)
                {
                    float targetDist = startOffset + (i * spacingBullets);

                    Vector3 posTarget = new Vector3(targetPos.x + targetDist, targetPos.y);
                    direction = (Vector2)posTarget - (Vector2)transform.position;
                    direction.Normalize();

                    GameObject go = Instantiate(m_bulletPrefab, fireCanon.position, Quaternion.identity);
                    go.GetComponent<PlayerProjectiles>().Direction = direction;
                    go.GetComponent<PlayerProjectiles>().playerRef = m_playerStats;
                }

                //SubBullets
                spacingBullets = 5f;
                nb = m_playerStats.GetNbSubBullets;
                startOffset = -((nb - 1) * spacingBullets) / 2f;

                for (int i = 0; i < nb; i++)
                {
                    float targetDist = startOffset + (i * spacingBullets);

                    Vector3 posTarget = new Vector3(targetPos.x + targetDist, targetPos.y);
                    direction = (Vector2)posTarget - (Vector2)transform.position;
                    direction.Normalize();

                    GameObject go = Instantiate(m_subBulletPrefab, fireCanon.position, Quaternion.identity);
                    go.GetComponent<PlayerProjectiles>().Direction = direction;
                    go.GetComponent<PlayerProjectiles>().playerRef = m_playerStats;
                }

                return true;
            }
        }
        return false;
    }

    private Transform GetFireCanon()
    {
        m_currentFireCanon++;
        if (m_currentFireCanon >= m_fireCanons.Count)
        {
            m_currentFireCanon = 0;
        }
        return m_fireCanons[m_currentFireCanon];
    }

    private void MoveFireCanon()
    {
        if (EnemyManager.Instance.Enemies.Count > 0)
        {
            Enemy target = EnemyManager.Instance.GetNearestEnemy(transform);

            for (int i = 0; i < m_arms.Count; i++)
            {
                Vector2 dir = target.transform.position - m_arms[i].position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion targetRot = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
                m_arms[i].rotation = Quaternion.Lerp(m_arms[i].rotation, targetRot, m_armRotationSpeed * Time.deltaTime);
            }
        }
    }
}