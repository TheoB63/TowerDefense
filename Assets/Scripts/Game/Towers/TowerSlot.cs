using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    //Need to delete color later
    public Color emptyColor = Color.gray;
    public Color occupiedColor = Color.green;

    [Header("Values")]
    [SerializeField] string m_slotName = "Slot";
    GameObject m_currentTower;
    SpriteRenderer m_slotRenderer;
    TowerType m_type;

    [Header("Public Values(Get only)")]
    public TowerType Type { get => m_type; }
    public bool HasTower() => m_currentTower != null;

    void Awake()
    {
        m_slotRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();
    }

    public void BuildTower(GameObject _towerPrefab, TowerType _type)
    {
        if (m_currentTower != null)
        {
            //This warning is normally impossible to attempt
            Debug.LogWarning($"The Slot {m_slotName} has a tower !");
            return;
        }

        m_currentTower = Instantiate(_towerPrefab, transform.position, Quaternion.identity);
        m_currentTower.transform.SetParent(transform);
        m_type = _type;
        UpdateVisual();
    }

    public ITower GetTower()
    {
        if (m_currentTower == null) return null;
        return m_currentTower.GetComponent<ITower>();
    }

    void UpdateVisual()
    {
        if (m_slotRenderer != null)
        {
            m_slotRenderer.color = HasTower() ? occupiedColor : emptyColor;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = HasTower() ? Color.green : Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
    }
}