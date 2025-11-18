using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_text;
    [SerializeField] float m_lifeTime;

    float m_timer;

    public void Setup(int _damage)
    {
        m_text.text = "-"+_damage.ToString();

    }

    private void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer > m_lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
