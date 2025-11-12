using UnityEngine;
using UnityEngine.Events;

public enum UpgradeCategory
{
    Player,
    Tower,
    Global
}

[CreateAssetMenu(fileName = "UpgradeTower", menuName = "Scriptable Objects/UpgradeTower")]

public class UpgradeTower : ScriptableObject
{
    [Header("Infos")]
    public string upgradeName;
    public string description;
    public UpgradeCategory category;

    [Header("Conditions")]
    public TowerSlot.Type targetTowerType = TowerSlot.Type.NONE; // Si réservé à une tour précise
    public bool requireTowerBuilt; // true = visible seulement si tour déjà construite

    [Header("Effet")]
    public int bonusDamage;
    public float bonusFireRate;

    public string label;
    public System.Action onSelect;
    public class ButtonClickedEvent : UnityEvent { }

    //public UpgradeTower(string _label, System.Action _onSelect)
    //{
    //    label = _label;
    //    onSelect = _onSelect;
    //}

    //public virtual void ApplyUpgrade(TowerManager towerManager)
    //{
    //    switch (category)
    //    {
    //        case UpgradeCategory.Player:
    //            Debug.Log($"Upgrade Player: {upgradeName}");
    //            //setPlayerEffects
    //            break;

    //        case UpgradeCategory.Tower:
    //            var slot = towerManager.GetSlotOfType(targetTowerType);
    //            if (slot != null && slot.Tower != null)
    //            {
    //                slot.UpgradeTower();
    //                Debug.Log($"Upgrade appliqué à {targetTowerType}");
    //            }
    //            break;

    //        case UpgradeCategory.Global:
    //            Debug.Log($"Upgrade Global: {upgradeName}");
    //            //setGlobalEffects
    //            break;
    //    }
    //}
}