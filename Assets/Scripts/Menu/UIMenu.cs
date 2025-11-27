using System.Linq;
using TMPro;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public static UIMenu Instance;

    [SerializeField] TextMeshProUGUI m_playerLevelText;
    [SerializeField] TextMeshProUGUI m_goldText;
    [SerializeField] TextMeshProUGUI m_diamondText;


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

    void OnEnable()
    {
        PlayerManager.Instance.OnPlayerLevelUp += UpdateUI;
        InventoryManager.Instance.OnGoldChanged += UpdateUI;
        InventoryManager.Instance.OnDiamondChanged += UpdateUI;
    }

    void OnDisable()
    {
        PlayerManager.Instance.OnPlayerLevelUp -= UpdateUI;
        InventoryManager.Instance.OnGoldChanged -= UpdateUI;
        InventoryManager.Instance.OnDiamondChanged -= UpdateUI;
    }

    void Start()
    {
        UpdateUI();
    }

    void UpdateUI(int _value = 0)
    {
        m_playerLevelText.text = "Lvl." + DataManager.Instance.playerManager.level.ToString();
        m_goldText.text = DataManager.Instance.inventoryManager.Gold.ToString() + " gold";
        m_diamondText.text = DataManager.Instance.inventoryManager.Diamond.ToString() + " diamond";
    }
}
