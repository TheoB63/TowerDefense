using TMPro;
using UnityEngine;

public class TowerStatPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_title;
    [SerializeField] TextMeshProUGUI m_values;

    public void Setup(string _title,string _values)
    {
        m_title.text = _title;
        m_values.text = _values;
    }
}
