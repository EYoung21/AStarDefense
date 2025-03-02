using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundInfoUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI difficultyText;
    public TextMeshProUGUI enemyScalingText;
    public TextMeshProUGUI nextWaveInfoText;
    
    [Header("Enemy Icons")]
    public Image[] enemyTypeIcons;
    public GameObject enemyInfoPanel;
    
    [Header("Settings")]
    public KeyCode toggleInfoKey = KeyCode.Tab;
    public float updateInterval = 1.0f;
    
    private float lastUpdateTime;
    private bool showingEnemyInfo = false;
    
    void Start()
    {
        // Hide enemy info panel initially
        if (enemyInfoPanel != null)
        {
            enemyInfoPanel.SetActive(false);
        }
        
        UpdateUI();
    }
    
    void Update()
    {
        // Update UI periodically
        if (Time.time > lastUpdateTime + updateInterval)
        {
            UpdateUI();
            lastUpdateTime = Time.time;
        }
        
        // Toggle enemy info panel
        if (Input.GetKeyDown(toggleInfoKey))
        {
            showingEnemyInfo = !showingEnemyInfo;
            if (enemyInfoPanel != null)
            {
                enemyInfoPanel.SetActive(showingEnemyInfo);
            }
        }
    }
    
    void UpdateUI()
    {
        if (RoundManager.Instance == null) return;
        
        int currentRound = RoundManager.Instance.round;
        string difficultyTier = RoundManager.Instance.GetDifficultyTierName();
        
        // Update round text
        if (roundText != null)
        {
            roundText.text = $"Round: {currentRound}";
        }
        
        // Update difficulty text
        if (difficultyText != null)
        {
            difficultyText.text = $"Difficulty: {difficultyTier}";
        }
        
        // Update enemy scaling text
        if (enemyScalingText != null)
        {
            float healthMultiplier = RoundManager.Instance.GetEnemyHealthMultiplier();
            float damageMultiplier = RoundManager.Instance.GetEnemyDamageMultiplier();
            
            if (healthMultiplier > 1.0f || damageMultiplier > 1.0f)
            {
                enemyScalingText.text = $"Enemy Scaling:\nHealth: {healthMultiplier:F2}x\nDamage: {damageMultiplier:F2}x";
            }
            else
            {
                enemyScalingText.text = "Enemy Scaling: None";
            }
        }
        
        // Update next wave info
        if (nextWaveInfoText != null && GameManager.Instance.GameState == GameState.PlayerPrepTurn)
        {
            int enemyCount = GameManager.Instance.globalNumberOfEnemiesToSpawn;
            nextWaveInfoText.text = $"Next Wave: {enemyCount} enemies";
            
            // Add info about potential special enemies
            if (currentRound >= 5)
            {
                nextWaveInfoText.text += "\nTitan may appear!";
            }
        }
    }
    
    // Call this method to show detailed info about a specific enemy type
    public void ShowEnemyTypeInfo(string enemyType)
    {
        string info = "Unknown Enemy";
        
        switch (enemyType)
        {
            case "BlueStar":
                info = "Basic Enemy\nHealth: 5\nDamage: 2\nSpeed: Medium";
                break;
            case "SpeedEnemy":
                info = "Speed Enemy\nHealth: 3\nDamage: 1\nSpeed: Very Fast";
                break;
            case "TankEnemy":
                info = "Tank Enemy\nHealth: 15\nDamage: 4\nSpeed: Slow";
                break;
            case "SoldierEnemy":
                info = "Soldier Enemy\nHealth: 7\nDamage: 2\nSpeed: Medium";
                break;
            case "TitanEnemy":
                info = "TITAN\nHealth: 30\nDamage: 8\nSpeed: Very Slow\nResists 20% damage";
                break;
        }
        
        // Display this info in a tooltip or panel
        Debug.Log(info);
    }
} 