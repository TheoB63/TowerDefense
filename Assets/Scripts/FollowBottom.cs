using UnityEngine;

public class FollowBottom : MonoBehaviour
{
    [Header("Values")]
    public float offsetY = 0.1f;

    //privates
    Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        Vector3 bottom = cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, cam.nearClipPlane));
        transform.position = new Vector3(transform.position.x, bottom.y + offsetY, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {        
        
    }
}
