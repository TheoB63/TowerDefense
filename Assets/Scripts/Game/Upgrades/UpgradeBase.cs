using Unity.VisualScripting;
using UnityEngine;

public enum UpgradeType
{
    Normal,
    LevelUp,
}

public abstract class UpgradeBase : ScriptableObject
{
    public string upgradeName;
    public string description;
    public int minLevel = 1;
    public int maxUtilisation = 1;
    public UpgradeBase link = null;
    public UpgradeType type;
    public Sprite icon;


    public abstract bool CanApply(TowerSlot _slot, PlayerStats _player);
    public abstract void Apply(TowerSlot _slot, PlayerStats _player);
    public abstract string GetDisplayText(TowerSlot _slot, PlayerStats _player);
}