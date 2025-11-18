using UnityEngine;

public class RailgunProjectile : Projectiles
{
    [Header("Values")]
    [SerializeField] float m_explosionRadius;
    [SerializeField] float m_explosionForce;
    int m_damage;
    int m_penetration;
    Vector2 direction;

    [Header("public values")]
    public Vector2 Direction { get => direction; set => direction = value; }

    void Update()
    {
        OutScreenDelete();

        if (direction != Vector2.zero)
        {
            Vector2 movement = direction * m_speed * Time.deltaTime;
            transform.position += new Vector3(movement.x, movement.y, 0f);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SetStats(int _damage, int _penetration)
    {
        m_damage = _damage;
        m_penetration = _penetration;
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.GetComponent<Enemy>())
        {
            _collision.GetComponent<Enemy>().TakeDamage(m_damage);

            m_penetration--;

            if (m_penetration < 0)
            {
                foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, m_explosionRadius))
                {
                    Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        Vector2 vec = _collision.transform.position - transform.position;
                        Vector2 direction = vec.normalized;
                        float distance = vec.magnitude;
                        float attenuation = (m_explosionRadius - distance) / m_explosionRadius;
                        rb.AddForce(direction * m_explosionForce * attenuation);
                    }
                }
                Destroy(this.gameObject);
            }
        }
    }
}