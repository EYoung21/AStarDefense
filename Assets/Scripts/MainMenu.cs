using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("GridDevTestScene"); //change game scene name
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked!"); //check if the function is triggered
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;  //stops Play Mode in the Editor
#else
        Application.Quit();  //quits the built game
#endif
    }
}

