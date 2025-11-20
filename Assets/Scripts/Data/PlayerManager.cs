using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    //Level
    public int level;
    public float xp = 0;
    public float xpNeeded;
    public float xpMultiplierNeeded;

    public event Action<int> OnPlayerLevelUp;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreateNewPlayer()
    {
        OnPlayerLevelUp?.Invoke(level);
    }

    public void AddXp(float _xp)
    {
        xp += _xp;
        if (xp >= xpNeeded)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        xp -= xpNeeded;
        xpNeeded *= xpMultiplierNeeded;
        OnPlayerLevelUp?.Invoke(level);
    }
}
