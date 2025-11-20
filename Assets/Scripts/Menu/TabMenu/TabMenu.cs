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
        for (int i = 0; i < m_tabs.Count; i++)
        {
            m_tabs[i].group = m_toggleGroup;
            m_tabs[i].isOn = i == m_pageIndex;
        }
        OpenPage(m_pageIndex);
    }

    private void Reset()
    {
        Initialized();
    }

    private void OnValidate()
    {
        Initialized();        
    }

    private void OpenPage(int _index)
    {
        for (int i = 0; i < m_pages.Count; i++)
        {
            bool isActivePage = (i == m_pageIndex);

            m_pages[i].gameObject.SetActive(isActivePage);
            m_pages[i].alpha = isActivePage ? 1f : 0f;
            m_pages[i].interactable = isActivePage;
            m_pages[i].blocksRaycasts = isActivePage;

            m_tabs[i].isOn = isActivePage;
        }

        foreach (var toggle in m_tabs)
        {
            var highlight = toggle.GetComponent<ToggleHighlightStaysActive>();
            highlight.RefreshHighlight();
        }
    }

    private void Awake()
    {
        foreach (Toggle toggle in m_tabs)
        {
            toggle.onValueChanged.AddListener(CheckForTab);
        }

        m_pageIndex = m_startPage;
        Initialized();
    }

    private void Start()
    {
        
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

    private void OnDestroy()
    {
        foreach (Toggle toggle in m_tabs)
        {
            toggle.onValueChanged.RemoveListener(CheckForTab);
            toggle.isOn = false;
        }
    }

    //public void JumpToPage(int _page)
    //{
    //    EnsureIndexIsInRange(_page);
    //    m_tabs[m_pageIndex].isOn = true;
    //}
}