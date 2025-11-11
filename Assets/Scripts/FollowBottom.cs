using UnityEngine;

public class FollowBottom : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float m_offsetY;

    //Camera
    Camera m_Camera;

    void Start()
    {
        m_Camera = Camera.main;
        Setup();
    }

    void Setup()
    {
        Vector3 bottom = m_Camera.ViewportToWorldPoint(new Vector3(0.5f, 0f, m_Camera.nearClipPlane));
        transform.position = new Vector3(transform.position.x, bottom.y + m_offsetY, transform.position.z);
    }
}
