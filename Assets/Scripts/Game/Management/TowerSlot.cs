using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    //[Header("Values")]
    //GameObject m_tower;

    [Header("Public Values")]
    public string slotName = "Slot";
    public Color emptyColor = Color.gray;
    public Color occupiedColor = Color.green;

    private GameObject currentTower;
    private SpriteRenderer slotRenderer;

    private void Awake()
    {
        slotRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();
    }

    public bool HasTower() => currentTower != null;

    public void BuildTower(GameObject towerPrefab)
    {
        if (currentTower != null)
        {
            Debug.LogWarning($"Le slot {slotName} a déjà une tourelle!");
            return;
        }

        currentTower = Instantiate(towerPrefab, transform.position, Quaternion.identity);
        currentTower.transform.SetParent(transform);
        UpdateVisual();
    }

    public ITower GetTower()
    {
        if (currentTower == null) return null;
        return currentTower.GetComponent<ITower>();
    }

    private void UpdateVisual()
    {
        if (slotRenderer != null)
        {
            slotRenderer.color = HasTower() ? occupiedColor : emptyColor;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = HasTower() ? Color.green : Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
    }
}
