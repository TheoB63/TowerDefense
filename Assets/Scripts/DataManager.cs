using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    //Level Data
    public LevelConfig selectedLevel;
    public float gameSpeed = 1f;

    //Player
    //level
    //

    //Tower

    //Inv
    //

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
}
