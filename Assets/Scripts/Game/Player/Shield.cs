using UnityEngine;
using UnityEngine.SceneManagement;

public class Shield : MonoBehaviour
{
    [SerializeField] float m_baseHp;
    float m_currentHp;

    public float BaseHp { get => m_baseHp; }
    public float Hp { get => m_currentHp; }

    private void Start()
    {
        m_currentHp = m_baseHp;
        UI.Instance.UpdateUI();
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.GetComponent<Enemy>())
        {
            m_currentHp -= _collision.GetComponent<Enemy>().Damage;

            //Here shield take damage
            _collision.GetComponent<Enemy>().Kill();
        }
        UI.Instance.UpdateUI();

        if (m_currentHp <= 0)
        {
            GameManager.Instance.EndGame(false);
        }
    }
}
