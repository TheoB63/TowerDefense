using UnityEngine;

public class Projectiles : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float m_speed;
    [SerializeField] int m_damageBase;
    int m_damage;
    int m_penetration;
    Vector2 direction;

    [Header("public values")]
    public Vector2 Direction { get => direction; set => direction = value; }

    void Start()
    {

    }

    void Update()
    {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>())
        {
            collision.GetComponent<Enemy>().TakeDamage(m_damageBase + m_damage);

            m_penetration--;

            if (m_penetration <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}