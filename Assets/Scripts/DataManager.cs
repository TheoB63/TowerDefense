using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    //Level Data
    public int selectedLevel = 1;

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
