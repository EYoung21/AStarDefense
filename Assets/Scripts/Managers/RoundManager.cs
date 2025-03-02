using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    [Header("Round Settings")]
    public int round = 1;
    public int difficultyTier = 1; // Increases every 5 rounds
    
    [Header("UI References")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI difficultyText;
    
    [Header("Enemy Scaling")]
    [Tooltip("How much enemy health increases per round after round 2")]
    public float healthScalingPerRound = 0.10f; // Reduced from 15% to 10% per round
    
    [Tooltip("How much enemy damage increases per round after round 3")]
    public float damageScalingPerRound = 0.08f; // Reduced from 10% to 8% per round
    
    [Tooltip("Maximum health multiplier cap")]
    public float maxHealthMultiplier = 5.0f; // Cap health scaling at 5x
    
    [Tooltip("Maximum damage multiplier cap")]
    public float maxDamageMultiplier = 4.0f; // Cap damage scaling at 4x
    
    [Tooltip("Round when health scaling begins")]
    public int healthScalingStartRound = 3;
    
    [Tooltip("Round when damage scaling begins")]
    public int damageScalingStartRound = 4;
    
    [Header("Round Milestones")]
    public int[] difficultyTierRounds = { 1, 5, 10, 15, 20 };
    public string[] difficultyTierNames = { "Novice", "Challenging", "Difficult", "Extreme", "Nightmare" };

    [Header("Round Rewards")]
    [Tooltip("Base currency reward for completing a round")]
    public int baseRoundCompletionReward = 50;
    
    [Tooltip("Additional currency per difficulty tier")]
    public int additionalRewardPerTier = 25;

    void Awake() {
        Instance = this;
    }
    
    void Start() {
        //initialize round display
        UpdateRoundDisplay();
    }

    public void IncrementRound(int amount) {
        int previousRound = round;
        round += amount;
        
        //check if we've reached a new difficulty tier
        int newDifficultyTier = CalculateDifficultyTier();
        if (newDifficultyTier > difficultyTier) {
            difficultyTier = newDifficultyTier;
            Debug.Log($"<color=orange>DIFFICULTY INCREASED: Now in {GetDifficultyTierName()} tier!</color>");
        }
        
        //log round change with scaling information
        Debug.Log($"<color=yellow>Round {previousRound} -> {round}</color>");
        Debug.Log($"Enemy Health Multiplier: {GetEnemyHealthMultiplier():F2}x");
        Debug.Log($"Enemy Damage Multiplier: {GetEnemyDamageMultiplier():F2}x");
        
        //update UI
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
        float multiplier = 1.0f + ((round - (healthScalingStartRound - 1)) * healthScalingPerRound);
        
        // Apply cap to prevent excessive scaling
        return Mathf.Min(multiplier, maxHealthMultiplier);
    }
    
    // Get enemy damage multiplier based on round number
    public float GetEnemyDamageMultiplier() {
        // Start increasing damage from damageScalingStartRound onwards
        if (round < damageScalingStartRound) return 1.0f;
        
        // Increase by damageScalingPerRound per round after damageScalingStartRound
        float multiplier = 1.0f + ((round - (damageScalingStartRound - 1)) * damageScalingPerRound);
        
        // Apply cap to prevent excessive scaling
        return Mathf.Min(multiplier, maxDamageMultiplier);
    }
    
    // Get round completion reward based on current difficulty tier
    public int GetRoundCompletionReward() {
        return baseRoundCompletionReward + ((difficultyTier - 1) * additionalRewardPerTier);
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
