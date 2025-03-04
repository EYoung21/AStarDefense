using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundInfoUI : MonoBehaviour
{
    public static RoundInfoUI Instance;

    [Header("UI References")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI difficultyText;
    public TextMeshProUGUI enemyScalingText;
    public TextMeshProUGUI nextWaveInfoText;
    
    [Header("Settings")]
    public KeyCode toggleInfoKey = KeyCode.Tab;
    public float updateInterval = 1.0f;
    
    private float lastUpdateTime;
    private bool showingRoundInfo = false;
    
    private void Awake() {
        Instance = this;
    }

    void Start()
    {
        //hide enemy info panel initially
        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }
        
        UpdateUI();
    }
    
    void Update()
    {
        //update UI periodically
        if (Time.time > lastUpdateTime + updateInterval)
        {
            UpdateUI();
            lastUpdateTime = Time.time;
        }
        
        //toggle enemy info panel
        if (Input.GetKeyDown(toggleInfoKey))
        {
            showingRoundInfo = !showingRoundInfo;
            if (gameObject != null)
            {
                gameObject.SetActive(showingRoundInfo);
            }
        }
    }

    public void setActiveAgain() {
        showingRoundInfo = true;
        gameObject.SetActive(showingRoundInfo);
    }
    
    void UpdateUI()
    {
        if (RoundManager.Instance == null) return;
        
        int currentRound = RoundManager.Instance.round;
        string difficultyTier = RoundManager.Instance.GetDifficultyTierName();
        
        //update round text
        if (roundText != null)
        {
            roundText.text = $"Round: {currentRound}";
        }
        
        //update difficulty text
        if (difficultyText != null)
        {
            difficultyText.text = $"Difficulty: {difficultyTier}";
        }
        
        //update enemy scaling text
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
        
        //update next wave info
        if (nextWaveInfoText != null && GameManager.Instance.GameState == GameState.PlayerPrepTurn)
        {
            int enemyCount = GameManager.Instance.globalNumberOfEnemiesToSpawn;
            nextWaveInfoText.text = $"Next Wave: {enemyCount} enemies";
            
            //add info about potential special enemies
            if (currentRound >= 5)
            {
                nextWaveInfoText.text += "\nTitan may appear!";
            }
        }
    }
} 