using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI highScoreText;  // Display the high score in the UI

    private int highScore = 0;

    void Start()
    {
        // Load high score from PlayerPrefs at the start of the game
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreDisplay();
    }

    public void CheckAndUpdateHighScore(int currentScore)
    {
        // If the current score is higher than the saved high score, update it
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);  // Save the new high score
            UpdateHighScoreDisplay();  // Update the UI
            Debug.Log($"New High Score: {highScore}");  // Log the new high score
        }
    }

    private void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {highScore}";
        }
    }
}
