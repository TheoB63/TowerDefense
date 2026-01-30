using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    [Header("references")]
    [SerializeField] Image m_background;
    [SerializeField] TextMeshProUGUI m_levelText;
    //[SerializeField] Image m_icon;

    [Header("temp values")]
    [SerializeField] TextMeshProUGUI m_tempIconText;
    [SerializeField] Color m_tempColorCommon;
    [SerializeField] Color m_tempColorRare;
    [SerializeField] Color m_tempColorEpic;
    [SerializeField] Color m_tempColorLegendary;

    public StuffItem stuff;

    private void Start()
    {
        switch (stuff.Rarity)
        {
            case Rarity.Common:
                m_background.color = m_tempColorCommon;
                break;
            case Rarity.Rare:
                m_background.color = m_tempColorRare;
                break;
            case Rarity.Epic:
                m_background.color = m_tempColorEpic;
                break;
            case Rarity.Legendary:
                m_background.color = m_tempColorLegendary;
                break;
            default:
                break;
        }

        m_levelText.text = "Lvl." + stuff.Level.ToString();

        switch (stuff.Type)
        {
            case StuffType.Helmet:
                m_tempIconText.text = "H";
                break;
            case StuffType.Plate:
                m_tempIconText.text = "P";
                break;
            case StuffType.Legs:
                m_tempIconText.text = "L";
                break;
            case StuffType.Boots:
                m_tempIconText.text = "B";
                break;
            case StuffType.Shield:
                m_tempIconText.text = "S";
                break;
            case StuffType.Canon:
                m_tempIconText.text = "C";
                break;
            default:
                break;
        }
    }
}
