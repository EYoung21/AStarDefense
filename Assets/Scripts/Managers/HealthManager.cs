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

    public float maxHealth = 20;

    public float health = 20;

    void Awake() {
        Instance = this;
    }
    
    void Start() {
        //override any Inspector values with our code values
        maxHealth = 20;
        health = 20;
        
        //update the UI to reflect the correct health value
        UIManager.Instance.updateHealthUI();
    }
    
    public void RemoveHealth(float amount) {
        health -= amount;
        //update the UI to reflect the new health value
        UIManager.Instance.updateHealthUI();
        if (health <= 0) {
            GameManager.Instance.GameState = GameState.GameOver;
        }
    }

    public void AddHealth(float amount) {
        health += amount;
        if (health > maxHealth) {
            health = maxHealth;
        }
        //update the UI to reflect the new health value
        UIManager.Instance.updateHealthUI();
    }
}
