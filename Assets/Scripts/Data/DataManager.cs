using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    [Header("Managers")]
    public PlayerManager playerManager;
    public UnlockManager unlockManager;

    [Header("Level")]
    public LevelConfig selectedLevel;
    public float gameSpeed = 1f;

    [Header("Towers")]
    public List<TowerStats> unlockedTowers = new List<TowerStats>();

    [Header("Upgrades")]
    public List<string> unlockedGameUpgrades = new List<string>();

    //Inv
    //


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            //GetManagers
            playerManager = GetComponent<PlayerManager>();
            unlockManager = GetComponent<UnlockManager>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadGame();
    }

    void LoadGame()
    {
        bool save = false; //need to replace with verification
        if (save)//save
        {
            LoadSave();
        }
        else
        {
            CreateSave();
        }
    }

    void CreateSave()
    {
        playerManager.CreateNewPlayer();
    }

    void LoadSave()
    {
        //load player

        //load unlockedTowers

        //load unlockedGameUpgrades

    }
}
