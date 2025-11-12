using UnityEngine;

public abstract class UpgradeBase : ScriptableObject
{
    public string upgradeName;
    public Sprite icon;
    public string description;

    public abstract bool CanApply(TowerSlot slot, PlayerStats player);
    public abstract void Apply(TowerSlot slot, PlayerStats player);
    public abstract string GetDisplayText(TowerSlot slot, PlayerStats player);
}