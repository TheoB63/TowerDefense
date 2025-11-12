using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField] TowerSlot[] slots;
    [SerializeField] GameObject[] towerPrefabs;

    public List<TowerSlot> Slots => slots.ToList();
    public List<UpgradeTower> AllUpgrades = new List<UpgradeTower>();

    public List<UpgradeTower> GetAvailableChoices()
    {
        List<UpgradeTower> choices = new List<UpgradeTower>();

        bool hasFreeSlot = slots.Any(slot => slot.TowerType == TowerSlot.Type.NONE);
        if (hasFreeSlot) 
        {
            foreach (TowerSlot.Type type in System.Enum.GetValues(typeof(TowerSlot.Type)))
            {
                if (type != TowerSlot.Type.NONE)
                {
                    bool alreadyBuilt = slots.Any(slot => slot.TowerType == type);
                    if (!alreadyBuilt)
                    {
                        //choices.Add(new UpgradeTower($"Construire {type}", () => BuildTower(type)));
                    }
                }                
            }
        }
        else
        {
            foreach (var slot in slots.Where(slot => slot.TowerType != TowerSlot.Type.NONE))
            {
                //choices.Add(new UpgradeTower($"Améliorer {slot.TowerType}", () => slot.UpgradeTower()));
            }
        }
        return choices;
    }

    void BuildTower(TowerSlot.Type type)
    {
        TowerSlot freeSlot = slots.FirstOrDefault(s => s.TowerType == TowerSlot.Type.NONE);
        if (freeSlot == null) return;

        int prefabIndex = (int)type - 1; // car BASIC_1 = 1
        GameObject towerPrefab = towerPrefabs[prefabIndex];

        freeSlot.BuildTower(type, towerPrefab);
    }

    public bool IsTowerBuilt(TowerSlot.Type type)
    {
        return Slots.Any(s => s.TowerType == type);
    }

    public TowerSlot GetSlotOfType(TowerSlot.Type type)
    {
        return Slots.FirstOrDefault(s => s.TowerType == type);
    }
}
