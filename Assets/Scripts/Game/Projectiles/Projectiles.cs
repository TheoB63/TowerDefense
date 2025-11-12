using UnityEngine;

public class Projectiles : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float m_speed;
    [SerializeField] int m_damage;
    Enemy m_target;

    [Header("public values")]
    public Enemy Target { get => m_target; set => m_target = value; }

    void Start()
    {

    }

    void Update()
    {
        if (m_target != null)
        {
            Vector2 targetPos = m_target.transform.position;
            Vector2 newPos = Vector2.Lerp(transform.position, targetPos, m_speed * Time.deltaTime);
            transform.position = newPos;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>())
        {
            collision.GetComponent<Enemy>().TakeDamage(m_damage);
            Destroy(this.gameObject);
        }
    }
}