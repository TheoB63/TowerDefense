using TMPro;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public static UIMenu Instance;

    [SerializeField] TextMeshProUGUI m_playerLevelText;

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

    void Start()
    {

    }

    void Update()
    {
        //need to remove later
        m_playerLevelText.text = "Lvl." + DataManager.Instance.playerManager.level.ToString();
    }
}
