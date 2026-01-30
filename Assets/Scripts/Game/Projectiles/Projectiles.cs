using Unity.VisualScripting;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] protected float m_speed;

    bool m_needDelete = false;
    protected virtual void OutScreenDelete()
    {
        if (m_needDelete)
        {
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        m_needDelete = true;
    }
}
