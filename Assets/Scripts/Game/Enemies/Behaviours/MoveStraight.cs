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
        ClampToScreen();
    }

    void ClampToScreen()
    {
        Camera m_cam = Camera.main;

        float halfWidth = GetComponent<BoxCollider2D>().bounds.extents.x;
        Vector3 left = m_cam.ViewportToWorldPoint(new Vector3(0, 0, m_cam.nearClipPlane));
        Vector3 right = m_cam.ViewportToWorldPoint(new Vector3(1, 0, m_cam.nearClipPlane));

        if (transform.position.x + halfWidth < left.x) //left
        {
            rb.linearVelocityX = enemy.Speed;
        }
        else if (transform.position.x - halfWidth > right.x) //right
        {
            rb.linearVelocityX = -enemy.Speed;
        }

        float halfHeight = GetComponent<BoxCollider2D>().bounds.extents.y;
        Vector3 top = m_cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, m_cam.nearClipPlane));
        Vector3 bottom = m_cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, m_cam.nearClipPlane));
        if (transform.position.y + halfHeight < bottom.y)
        {
            transform.position = new Vector3(top.x,top.y + halfHeight,transform.position.z);
        }
    }
}
