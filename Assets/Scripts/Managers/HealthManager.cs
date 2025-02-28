using UnityEngine;

public class HealthManager : MonoBehaviour
{
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public static HealthManager Instance;

    public float maxHealth = 100;

    void Awake() {
        Instance = this;
    }

    public float health = 100;
    
    public void RemoveHealth(float amount) {
        health -= amount;
        if (health <= 0) {
            GameManager.Instance.GameState = GameState.GameOver;
        }
    }

    public void AddHealth(float amount) {
        health += amount;
        if (health > maxHealth) {
            health = maxHealth;
        }
    }
}
