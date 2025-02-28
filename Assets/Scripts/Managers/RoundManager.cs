using UnityEngine;

public class RoundManager : MonoBehaviour
{
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public static RoundManager Instance;

    void Awake() {
        Instance = this;
    }  

    public int round = 0; //we will increment round as rounds end

    public void IncrementRound(int amount) {
        round += amount;
    }
    
    // Get enemy health multiplier based on round number
    public float GetEnemyHealthMultiplier() {
        // Start increasing health from round 3 onwards
        if (round <= 2) return 1.0f;
        
        // Increase by 15% per round after round 2
        return 1.0f + ((round - 2) * 0.15f);
    }
    
    // Get enemy damage multiplier based on round number
    public float GetEnemyDamageMultiplier() {
        // Start increasing damage from round 4 onwards
        if (round <= 3) return 1.0f;
        
        // Increase by 10% per round after round 3
        return 1.0f + ((round - 3) * 0.1f);
    }
}
