using UnityEngine;
using UnityEngine.SceneManagement; // Add this for scene management

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance;

    public float maxHealth = 15;
    public float health = 15;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        maxHealth = 15;
        health = 15;
        UIManager.Instance.updateHealthUI();
    }

    public void RemoveHealth(float amount)
    {
        health -= amount;
        UIManager.Instance.updateHealthUI();

        if (health <= 0)
        {
            Debug.Log("Player has died! Loading Game Over Scene...");
            SceneManager.LoadScene("GameOverScene"); 
        }
    }

    public void AddHealth(float amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        UIManager.Instance.updateHealthUI();
    }
}

