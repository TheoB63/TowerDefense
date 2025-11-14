using UnityEngine;

public class MoveStraight : MonoBehaviour
{
    Enemy enemy;
    Rigidbody2D rb;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();    
    }

    void Update()
    {
        rb.linearVelocity = new Vector3(0f, -enemy.Speed, 0f);
    }
}
