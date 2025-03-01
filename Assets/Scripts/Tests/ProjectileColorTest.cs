using UnityEngine;

public class ProjectileColorTest : MonoBehaviour
{
    [Header("References")]
    public BaseTurret testTurret;
    
    [Header("Test Controls")]
    public KeyCode testFrostKey = KeyCode.F;
    public KeyCode testPoisonKey = KeyCode.P;
    public KeyCode testSplashKey = KeyCode.S;
    public KeyCode testRapidFireKey = KeyCode.R;
    public KeyCode testSniperKey = KeyCode.N;
    public KeyCode resetKey = KeyCode.X;
    
    void Update()
    {
        if (testTurret == null)
        {
            // Try to find a turret if none is assigned
            testTurret = FindFirstObjectByType<BaseTurret>();
            if (testTurret == null)
            {
                Debug.LogWarning("No turret found for testing");
                return;
            }
        }
        
        // Test frost effect
        if (Input.GetKeyDown(testFrostKey))
        {
            Debug.Log("Testing Frost Effect");
            testTurret.UpdateEffects(0.5f, 0f, 0f, 0f, 0f);
        }
        
        // Test poison effect
        if (Input.GetKeyDown(testPoisonKey))
        {
            Debug.Log("Testing Poison Effect");
            testTurret.UpdateEffects(0f, 4f, 0f, 0f, 0f);
        }
        
        // Test splash effect
        if (Input.GetKeyDown(testSplashKey))
        {
            Debug.Log("Testing Splash Effect");
            testTurret.UpdateEffects(0f, 0f, 1.5f, 0.5f, 0f);
        }
        
        // Test rapid fire effect
        if (Input.GetKeyDown(testRapidFireKey))
        {
            Debug.Log("Testing Rapid Fire Effect");
            testTurret.UpdateStats(1.0f, 1.0f, 1.5f);
        }
        
        // Test sniper effect
        if (Input.GetKeyDown(testSniperKey))
        {
            Debug.Log("Testing Sniper Effect");
            testTurret.UpdateStats(1.5f, 1.3f, 1.0f);
        }
        
        // Reset all effects
        if (Input.GetKeyDown(resetKey))
        {
            Debug.Log("Resetting All Effects");
            testTurret.UpdateEffects(0f, 0f, 0f, 0f, 0f);
            testTurret.UpdateStats(1.0f, 1.0f, 1.0f);
        }
    }
} 