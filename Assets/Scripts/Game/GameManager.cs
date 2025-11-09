using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static public GameManager Instance;

    [Header("Enemies")]
    public List<GameObject> enemiesType;

    public List<Enemy> enemies;

    //spawn test
    float maxTimer = 1;
    float timer;

    private void Awake()
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

    void Start()
    {
        
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > maxTimer)
        {
            foreach (GameObject enemy in enemiesType)
            {
                float x = Random.Range(0, Screen.width);
                Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(x, Screen.height, Camera.main.nearClipPlane));
                GameObject go = Instantiate(enemy, position, Quaternion.identity);
                enemies.Add(go.GetComponent<Enemy>());
                timer -= maxTimer;
            }
        }
    }
}
