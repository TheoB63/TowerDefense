using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }
}
