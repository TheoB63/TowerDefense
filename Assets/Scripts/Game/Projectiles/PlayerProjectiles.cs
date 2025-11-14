using UnityEngine;

public class PlayerProjectiles : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float m_speed;
    [SerializeField] int m_damageBase;

    //runtime values
    int m_damage;
    int m_penetration;
    Vector2 m_direction;

    [Header("public values")]
    public Vector2 Direction { get => m_direction; set => m_direction = value; }

    void Update()
    {
        if (m_direction != Vector2.zero)
        {
            Vector2 movement = m_direction * m_speed * Time.deltaTime;
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
            _collision.GetComponent<Enemy>().TakeDamage(m_damageBase + m_damage);

            m_penetration--;

            if (m_penetration < 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}