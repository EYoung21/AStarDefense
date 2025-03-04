using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;

    private int highScore;

    //ensure this is a singleton
    public static ScoreManager Instance;

    void Awake()
    {
        //if an instance already exists, destroy this one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            //otherwise, set this instance
            Instance = this;
            
            //check if this is a root GameObject before using DontDestroyOnLoad
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);  //this makes the ScoreManager persist across scenes
            }
            else
            {
                //if not a root GameObject, detach from parent and then use DontDestroyOnLoad
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
        }

        // Initialize high score (could be loaded from PlayerPrefs if you want persistence)
        highScore = PlayerPrefs.GetInt("HighScore", 1);  // default to 1 if not set
        UpdateHighScoreUI();
    }

    public int GetHighScore()
    {
        return highScore;
    }

    public void SetHighScore(int newHighScore)
    {
        highScore = newHighScore;
        PlayerPrefs.SetInt("HighScore", highScore);  // Save to PlayerPrefs
        UpdateHighScoreUI();
    }

    private void UpdateHighScoreUI()
    {
        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: Round {highScore}";
        }
    }
}
