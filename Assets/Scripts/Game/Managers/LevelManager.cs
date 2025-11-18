using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //Instance
    static public LevelManager Instance;

    [Header("Level Values")]
    [SerializeField] float m_baseXpNeeded;
    [SerializeField] float m_xpNeededMultiplier;
    float m_xpNeeded = 1;
    int m_level = 0;
    float m_xp = 0;

    [Header("Public Values(Get only)")]
    public int Level { get => m_level; }
    public float Xp { get => m_xp; }
    public float XpNeeded { get => m_xpNeeded; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        m_xpNeeded = m_baseXpNeeded;
    }

    public void AddXp(float _xp)
    {
        m_xp += _xp;
        LevelUp();
        UI.Instance.UpdateUI();
    }

    void LevelUp()
    {
        if (m_xp >= m_xpNeeded)
        {
            m_level++;
            m_xp -= m_xpNeeded;
            m_xpNeeded += m_baseXpNeeded * m_xpNeededMultiplier;
            GetComponent<UpgradeChoiceManager>().OnLevelUp();
        }
    }
}