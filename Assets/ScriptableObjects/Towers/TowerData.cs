using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Objects/TowerData")]
public class TowerData : ScriptableObject
{
    public TowerSlot.Type Type;
    public GameObject Tower;
    public List<UpgradeTower> Upgrades = new List<UpgradeTower>();
}