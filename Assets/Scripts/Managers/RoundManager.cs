using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Round Settings")]
    public int round = 0;
    public int difficultyTier = 1; // Increases every 5 rounds
    
    [Header("UI References")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI difficultyText;
    
    [Header("Enemy Scaling")]
    [Tooltip("How much enemy health increases per round after round 2")]
    public float healthScalingPerRound = 0.15f; // 15% per round
    
    [Tooltip("How much enemy damage increases per round after round 3")]
    public float damageScalingPerRound = 0.10f; // 10% per round
    
    [Tooltip("Round when health scaling begins")]
    public int healthScalingStartRound = 3;
    
    [Tooltip("Round when damage scaling begins")]
    public int damageScalingStartRound = 4;
    
    [Header("Round Milestones")]
    public int[] difficultyTierRounds = { 1, 5, 10, 15, 20 };
    public string[] difficultyTierNames = { "Novice", "Challenging", "Difficult", "Extreme", "Nightmare" };

    void Awake() {
        Instance = this;
    }
    
    void Start() {
        // Initialize round display
        UpdateRoundDisplay();
    }

    public void IncrementRound(int amount) {
        int previousRound = round;
        round += amount;
        
        // Check if we've reached a new difficulty tier
        int newDifficultyTier = CalculateDifficultyTier();
        if (newDifficultyTier > difficultyTier) {
            difficultyTier = newDifficultyTier;
            Debug.Log($"<color=orange>DIFFICULTY INCREASED: Now in {GetDifficultyTierName()} tier!</color>");
        }
        
        // Log round change with scaling information
        Debug.Log($"<color=yellow>Round {previousRound} -> {round}</color>");
        Debug.Log($"Enemy Health Multiplier: {GetEnemyHealthMultiplier():F2}x");
        Debug.Log($"Enemy Damage Multiplier: {GetEnemyDamageMultiplier():F2}x");
        
        // Update UI
        UpdateRoundDisplay();
    }
    
    private void UpdateRoundDisplay() {
        if (roundText != null) {
            roundText.text = $"Round: {round}";
        }
        
        if (difficultyText != null) {
            difficultyText.text = $"Difficulty: {GetDifficultyTierName()}";
        }
    }
    
    private int CalculateDifficultyTier() {
        for (int i = difficultyTierRounds.Length - 1; i >= 0; i--) {
            if (round >= difficultyTierRounds[i]) {
                return i + 1;
            }
        }
        return 1;
    }
    
    public string GetDifficultyTierName() {
        int tier = CalculateDifficultyTier();
        if (tier >= 0 && tier < difficultyTierNames.Length) {
            return difficultyTierNames[tier];
        }
        return "Unknown";
    }
    
    // Get enemy health multiplier based on round number
    public float GetEnemyHealthMultiplier() {
        // Start increasing health from healthScalingStartRound onwards
        if (round < healthScalingStartRound) return 1.0f;
        
        // Increase by healthScalingPerRound per round after healthScalingStartRound
        return 1.0f + ((round - (healthScalingStartRound - 1)) * healthScalingPerRound);
    }
    
    // Get enemy damage multiplier based on round number
    public float GetEnemyDamageMultiplier() {
        // Start increasing damage from damageScalingStartRound onwards
        if (round < damageScalingStartRound) return 1.0f;
        
        // Increase by damageScalingPerRound per round after damageScalingStartRound
        return 1.0f + ((round - (damageScalingStartRound - 1)) * damageScalingPerRound);
    }
    
    // Get a description of the current round's difficulty
    public string GetRoundDifficultyDescription() {
        string description = $"Round {round}: {GetDifficultyTierName()} Difficulty\n";
        
        if (round < healthScalingStartRound && round < damageScalingStartRound) {
            description += "Enemies have normal stats.";
        } else {
            if (round >= healthScalingStartRound) {
                description += $"Enemy Health: +{((GetEnemyHealthMultiplier() - 1) * 100):F0}%\n";
            }
            
            if (round >= damageScalingStartRound) {
                description += $"Enemy Damage: +{((GetEnemyDamageMultiplier() - 1) * 100):F0}%";
            }
        }
        
        return description;
    }
}
