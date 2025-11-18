using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void Resume()
    {
        GameManager.Instance.ResumeGame();
        gameObject.SetActive(false);
    }

    public void Quit()
    {
        GameManager.Instance.EndGame(false);
        gameObject.SetActive(false);
    }
}
