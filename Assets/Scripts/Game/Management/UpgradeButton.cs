using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Button button;

    private UpgradeBase upgrade;
    private TowerSlot targetSlot;
    private UpgradeChoiceManager manager;

    private void Awake()
    {
        // Récupérer le bouton automatiquement si non assigné
        if (button == null)
            button = GetComponent<Button>();
    }

    public void Setup(UpgradeBase upgrade, TowerSlot slot, UpgradeChoiceManager manager)
    {
        this.upgrade = upgrade;
        this.targetSlot = slot;
        this.manager = manager;

        // Mettre à jour l'UI
        if (iconImage != null && upgrade.icon != null)
            iconImage.sprite = upgrade.icon;

        if (nameText != null)
            nameText.text = upgrade.upgradeName;

        if (descriptionText != null)
            descriptionText.text = upgrade.description;

        // Ajouter le listener
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        manager.ApplyUpgrade(upgrade, targetSlot);
    }
}