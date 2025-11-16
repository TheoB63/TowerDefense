using UnityEngine;
using UnityEngine.SceneManagement;

public class Shield : MonoBehaviour
{
    [SerializeField] int m_baseHp;
    int m_currentHp;

    public int BaseHp { get => m_baseHp;}
    public int Hp { get => m_currentHp; }

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
            GameManager.Instance.EndLevel(false);
        }
    }
}
