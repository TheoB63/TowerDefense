using UnityEngine;

public class Drone : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] GameObject m_projectilePrefab;
    [SerializeField] float m_speed;
    [SerializeField] float m_attackSpeed;
    [SerializeField] float m_range;

    //private values
    int m_currentNbAttack;
    float m_damage;

    bool isInRange;

    //Timer
    float m_timerShoot;

    public void SetStats(float _damage, int _nbAttack)
    {
        m_damage = _damage;
        m_currentNbAttack = _nbAttack;
    }

    void Update()
    {
        Move();
        if (isInRange)
        {
            m_timerShoot += Time.deltaTime;
            if (m_timerShoot > (1 / m_attackSpeed))
            {
                Attack();
                m_timerShoot = 0;
                m_currentNbAttack--;
            }
        }
        if (m_currentNbAttack <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void Move()
    {
        isInRange = false;

        if (EnemyManager.Instance.Enemies.Count > 0)
        {
            Enemy target = EnemyManager.Instance.GetNearestEnemy(transform);
            float dist = Vector2.Distance(transform.position, target.transform.position);
            if (dist <= m_range)
            {
                isInRange = true;
            }
            else
            {
                Vector2 targetPos = target.transform.position;
                Vector3 posTarget = new Vector3(targetPos.x, targetPos.y);

                Vector2 direction;
                direction = posTarget - transform.position;
                direction.Normalize();

                Vector3 movement = new Vector3(direction.x, direction.y, 0);
                transform.position += movement * m_speed * Time.deltaTime;
            }
        }
    }

    private void Attack()
    {
        Enemy target = EnemyManager.Instance.GetNearestEnemy(transform);
        Vector2 targetPos = target.transform.position;


        Vector3 posTarget = new Vector3(targetPos.x, targetPos.y);

        Vector2 direction;
        direction = posTarget - transform.position;
        direction.Normalize();

        GameObject go = Instantiate(m_projectilePrefab, transform.position, Quaternion.identity);
        go.GetComponent<DroneProjectiles>().Direction = direction;
        go.GetComponent<DroneProjectiles>().SetStats(m_damage, 0);
    }
}
