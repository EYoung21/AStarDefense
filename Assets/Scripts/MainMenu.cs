using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("GridDevTestScene"); // Change game scene name
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked!"); // Check if the function is triggered
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;  // Stops Play Mode in the Editor
#else
        Application.Quit();  // Quits the built game
#endif
    }
}

