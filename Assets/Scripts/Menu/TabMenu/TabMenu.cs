using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TabMenu : MonoBehaviour
{
    [Header("Index")]
    [SerializeField] int m_pageIndex;
    [SerializeField] int m_startPage;

    [Header("Components")]
    [SerializeField] ToggleGroup m_toggleGroup;
    [SerializeField] List<Toggle> m_tabs = new List<Toggle>();
    [SerializeField] List<CanvasGroup> m_pages = new List<CanvasGroup>();

    [Header("Event to call")]
    public UnityEvent<int> OnPageIndexChanged;

    private void Initialized()
    {
        //m_toggleGroup = GetComponentInChildren<ToggleGroup>();

        //m_tabs.Clear();
        //m_pages.Clear();

        //m_tabs.AddRange(GetComponentsInChildren<Toggle>());
        //m_pages.AddRange(GetComponentsInChildren<CanvasGroup>());
    }

    private void Reset()
    {
        Initialized();
    }

    private void OnValidate()
    {
        Initialized();
        OpenPage(m_pageIndex);
        m_tabs[m_pageIndex].SetIsOnWithoutNotify(true);
    }

    private void Awake()
    {
        foreach (Toggle toggle in m_tabs)
        {
            toggle.onValueChanged.AddListener(CheckForTab);
            toggle.group = m_toggleGroup;
        }
    }

    private void Start()
    {
        EnsureIndexIsInRange(m_startPage);
        m_pageIndex = m_startPage;
        m_tabs[m_pageIndex].SetIsOnWithoutNotify(true);
        OpenPage(m_pageIndex);
    }

    private void OnDestroy()
    {
        foreach (Toggle toggle in m_tabs)
        {
            toggle.onValueChanged.RemoveListener(CheckForTab);
        }
    }

    private void CheckForTab(bool _value)
    {
        for (int i = 0; i < m_tabs.Count; i++)
        {
            if (!m_tabs[i].isOn)
            {
                continue;
            }
            m_pageIndex = i;
        }
        OpenPage(m_pageIndex);
    }

    private void OpenPage(int _index)
    {
        EnsureIndexIsInRange(_index);

        for (int i = 0; i < m_pages.Count; i++)
        {
            bool isActivePage = (i == m_pageIndex);

            m_pages[i].alpha = isActivePage ? 1 : 0;
            m_pages[i].interactable = isActivePage;
            m_pages[i].blocksRaycasts = isActivePage;
        }

        foreach (var toggle in m_tabs)
        {
            var highlight = toggle.GetComponent<ToggleHighlightStaysActive>();
            if (highlight != null)
                highlight.RefreshHighlight();
        }

        if (Application.isPlaying)
        {
            OnPageIndexChanged?.Invoke(m_pageIndex);
        }
    }

    private void EnsureIndexIsInRange(int _index)
    {
        if (m_tabs.Count == 0 || m_pages.Count == 0)
        {
            return;
        }

        m_pageIndex = Mathf.Clamp(_index, 0, m_pages.Count - 1);
    }

    public void JumpToPage(int _page)
    {
        EnsureIndexIsInRange(_page);
        m_tabs[m_pageIndex].isOn = true;
    }
}
