using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    //Level Data
    public LevelConfig selectedLevel;

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
