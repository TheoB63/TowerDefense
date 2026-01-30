using UnityEngine;

public class PlayerProjectiles : Projectiles
{
    public PlayerStats playerRef;

    //runtime values
    int m_penetration;
    Vector2 m_direction;

    [Header("public values")]
    public Vector2 Direction { get => m_direction; set => m_direction = value; }

    private void Start()
    {
        m_penetration = playerRef.GetPenetration;
    }

    void Update()
    {
        OutScreenDelete();

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

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.GetComponent<Enemy>())
        {
            _collision.GetComponent<Enemy>().TakeDamage(playerRef.GetDamage);

            m_penetration--;

            if (m_penetration < 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}