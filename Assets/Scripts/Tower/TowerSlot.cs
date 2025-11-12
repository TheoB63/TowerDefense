using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    public enum Type
    {
        NONE,
        BASIC_1,
        BASIC_2,
        BASIC_3,
        BASIC_4,
    }

    [Header("Values")]
    [SerializeField] Type m_type = Type.NONE;
    GameObject m_tower;

    [Header("Public Values")]
    public GameObject Tower { get => m_tower; set => m_tower = value; }
    
    [Header("Public Values (Get only)")]
    public Type TowerType { get => m_type;}

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void BuildTower(Type type, GameObject prefab)
    {
        if (m_type != Type.NONE)
            return;

        m_tower = Instantiate(prefab, transform.position, Quaternion.identity, transform);
        m_type = type;
        Debug.Log($"Tour construite : {m_type}");
    }

    public void UpgradeTower()
    {
        if (m_tower == null)
            return;

        Debug.Log($"Upgrade appliqué à {m_type}");
        // Ici tu pourras plus tard déclencher un script spécifique à la tour
    }
}
