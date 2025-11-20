using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if (true)//here loading end
        {
            SceneManager.LoadScene("Menu");
        }
    }
}