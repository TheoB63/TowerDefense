using UnityEngine;
using UnityEngine.UI;

public class ToggleHighlightStaysActive : MonoBehaviour
{
    [SerializeField] Toggle m_toggle;
    [SerializeField] Image m_imageKeepFocus;

    private void Reset()
    {
        m_toggle = GetComponent<Toggle>();
    }

    private void Awake()
    {
        m_toggle.onValueChanged.AddListener(OnToggleValueChanged);
        OnToggleValueChanged(m_toggle.isOn);
    }

    private void OnDestroy()
    {
        m_toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool _isOn)
    {
        if (m_imageKeepFocus == null)
        {
            return;
        }

        m_imageKeepFocus.color = m_toggle.isOn ? m_toggle.colors.highlightedColor : Color.clear;
    }

    public void RefreshHighlight()
    {
        OnToggleValueChanged(m_toggle.isOn);
    }
}
