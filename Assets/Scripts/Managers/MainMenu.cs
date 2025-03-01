using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Ethan Development"); // Change game scene name
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

