using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float speed;

    //Physics
    Collider2D boxCollider;

    //Inputs
    InputAction touchInput;
    InputAction touchPosInput;

    //Camera
    Camera cam;

    //Movement and Touch
    bool isTouching;
    Vector2 targetWorldPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boxCollider = GetComponent<Collider2D>();
        touchInput = InputSystem.actions.FindAction("Touch Press");
        touchPosInput = InputSystem.actions.FindAction("Touch Position");
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        isTouching = touchInput.IsPressed();
        if (isTouching)
        {
            //world pos
            Vector2 screenPos = touchPosInput.ReadValue<Vector2>();
            targetWorldPos = cam.ScreenToWorldPoint(screenPos);

            float diff = targetWorldPos.x - transform.position.x;
            float dir = Mathf.Sign(diff);

            Vector2 newPos = Vector2.Lerp(transform.position, targetWorldPos, speed * Time.deltaTime);

            transform.position = new Vector2(newPos.x, transform.position.y);          
        }        

        ClampToScreen();
    }

    void ClampToScreen()
    {
        float halfWidth = boxCollider.bounds.extents.x;
        Vector3 left = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 right = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));

        float clampedX = Mathf.Clamp(transform.position.x, left.x + halfWidth, right.x - halfWidth);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }
}
