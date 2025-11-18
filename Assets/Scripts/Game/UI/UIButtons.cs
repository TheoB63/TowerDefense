using UnityEngine;

public class UIButtons : MonoBehaviour
{
    [SerializeField] GameObject m_pauseMenuPanel;

    public void OpenPauseMenu()
    {
        GameManager.Instance.PauseGame();
        m_pauseMenuPanel.SetActive(true);
    }

    public void SetGameSpeed()
    {

    }

    public void OpenUpgradeMenu()
    {

    }

    public void ChangeTargetType()
    {

    }
}
