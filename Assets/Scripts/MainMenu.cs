using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene"); // Change game scene name
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

