using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerLevelUpEvent
{
    public int level;
    public UnityEvent onLevelUp; 
}

public class PlayerData : MonoBehaviour
{
    //Level
    public int level;
    public float xp = 0;
    public float xpNeeded = 2;
    public float baseXpNeeded = 2;
    public float xpMultiplierNeeded = 1.5f;

    public List<PlayerLevelUpEvent> levelUpEvents;

    //public static event Action<int> OnPlayerLevelUp;

    public void AddXp(float _xp)
    {
        xp += _xp;
        
    }

    public void LevelUp()
    {
        level++;
        xp -= xpNeeded;
        xpNeeded += baseXpNeeded * xpMultiplierNeeded;
        CheckLevelUpEvents();
    }

    void CheckLevelUpEvents()
    {
        foreach (var evt in levelUpEvents)
        {
            if (evt.level == level)
            {
                evt.onLevelUp?.Invoke();
            }
        }
    }
}
