using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float m_speed;

    //Physics
    Collider2D m_boxCollider;

    //Inputs
    InputAction m_touchInput;
    InputAction m_touchPosInput;

    //Camera
    Camera m_cam;

    //Movement and Touch
    bool m_isTouching;
    Vector2 m_targetWorldPos;

    void Start()
    {
        m_boxCollider = GetComponent<Collider2D>();
        m_touchInput = InputSystem.actions.FindAction("Touch Press");
        m_touchPosInput = InputSystem.actions.FindAction("Touch Position");
        m_cam = Camera.main;
    }

    void Update()
    {
        m_isTouching = m_touchInput.IsPressed();
        if (m_isTouching)
        {
            //world pos
            Vector2 screenPos = m_touchPosInput.ReadValue<Vector2>();
            m_targetWorldPos = m_cam.ScreenToWorldPoint(screenPos);

            float diff = m_targetWorldPos.x - transform.position.x;
            float dir = Mathf.Sign(diff);

            Vector2 newPos = Vector2.Lerp(transform.position, m_targetWorldPos, m_speed * Time.deltaTime);

            transform.position = new Vector2(newPos.x, transform.position.y);          
        }        

        ClampToScreen();
    }

    void ClampToScreen()
    {
        float halfWidth = m_boxCollider.bounds.extents.x;
        Vector3 left = m_cam.ViewportToWorldPoint(new Vector3(0, 0, m_cam.nearClipPlane));
        Vector3 right = m_cam.ViewportToWorldPoint(new Vector3(1, 0, m_cam.nearClipPlane));

        float clampedX = Mathf.Clamp(transform.position.x, left.x + halfWidth, right.x - halfWidth);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }
}
