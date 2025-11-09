using UnityEngine;

public class Projectiles : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] int damage;
    public Enemy target;

    void Start()
    {

    }

    void Update()
    {
        if (target != null)
        {
            Vector2 targetPos = target.transform.position;
            Vector2 newPos = Vector2.Lerp(transform.position, targetPos, speed * Time.deltaTime);
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
            collision.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(this.gameObject);
        }
    }
}