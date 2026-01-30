using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] Image m_background;
    [SerializeField] Image m_iconImage;
    [SerializeField] TextMeshProUGUI m_nameText;
    [SerializeField] TextMeshProUGUI m_descriptionText;
    [SerializeField] Button m_button;

    [Header("TempColor")]
    [SerializeField] Color tempNormalColor;
    [SerializeField] Color tempLevelUpColor;

    UpgradeBase m_upgrade;
    TowerSlot m_targetSlot;
    UpgradeChoiceManager m_manager;

    void Awake()
    {
        if (m_button == null)
            m_button = GetComponent<Button>();
    }

    public void Setup(UpgradeBase _upgrade, TowerSlot _slot, UpgradeChoiceManager _manager)
    {
        m_upgrade = _upgrade;
        m_targetSlot = _slot;
        m_manager = _manager;

        //Update UI
        if (m_iconImage != null && _upgrade.icon != null)
            m_iconImage.sprite = _upgrade.icon;

        if (m_nameText != null)
            m_nameText.text = _upgrade.upgradeName;

        if (m_descriptionText != null)
            m_descriptionText.text = _upgrade.description;

        if (_upgrade.type == UpgradeType.Normal)
        {
            m_background.color = tempNormalColor;
        }
        else if (_upgrade.type == UpgradeType.LevelUp)
        {
            m_background.color = tempLevelUpColor;
        }

        //Add listener
        if (m_button != null)
        {
            m_button.onClick.RemoveAllListeners();
            m_button.onClick.AddListener(OnButtonClicked);
        }
    }

    void OnButtonClicked()
    {
        m_manager.ApplyUpgrade(m_upgrade, m_targetSlot);
    }
}