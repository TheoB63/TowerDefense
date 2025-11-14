using UnityEngine;

public class Shield : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.GetComponent<Enemy>())
        {
            //Here shield take damage
            GameManager.Instance.Enemies.Remove(_collision.GetComponent<Enemy>());
            Destroy(_collision.gameObject);
        }
    }
}
