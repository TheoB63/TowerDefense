using UnityEngine;

public abstract class UpgradeBase : ScriptableObject
{
    public string upgradeName;
    public Sprite icon;
    public string description;

    public abstract bool CanApply(TowerSlot _slot, PlayerStats _player);
    public abstract void Apply(TowerSlot _slot, PlayerStats _player);
    public abstract string GetDisplayText(TowerSlot _slot, PlayerStats _player);
}