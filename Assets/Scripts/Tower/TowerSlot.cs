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

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
