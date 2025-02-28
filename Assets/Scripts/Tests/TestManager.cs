using UnityEngine;

public class TestManager : MonoBehaviour
{
    [SerializeField] private bool enableCurrencyTest = true;
    [SerializeField] private bool enableTurretUpgradeTest = true;
    
    void Awake()
    {
        Debug.Log("TestManager Awake - Setting up test components");
        
        if (enableCurrencyTest)
        {
            if (GetComponent<CurrencyTest>() == null)
            {
                gameObject.AddComponent<CurrencyTest>();
                Debug.Log("Added CurrencyTest component");
            }
        }
        
        if (enableTurretUpgradeTest)
        {
            if (GetComponent<TurretUpgradeTest>() == null)
            {
                gameObject.AddComponent<TurretUpgradeTest>();
                Debug.Log("Added TurretUpgradeTest component");
            }
        }
    }
    
    void Start()
    {
        // Print important debug information
        Debug.Log("TestManager Start - Printing debug information");
        
        // Check CurrencyManager
        if (CurrencyManager.Instance != null)
        {
            Debug.Log($"CurrencyManager.Instance exists with currency: {CurrencyManager.Instance.currency}");
        }
        else
        {
            Debug.LogError("CurrencyManager.Instance is null!");
        }
        
        // Check UIManager
        if (UIManager.Instance != null)
        {
            Debug.Log("UIManager.Instance exists");
        }
        else
        {
            Debug.LogError("UIManager.Instance is null!");
        }
        
        // Check TurretUpgradeUI
        if (TurretUpgradeUI.Instance != null)
        {
            Debug.Log("TurretUpgradeUI.Instance exists");
        }
        else
        {
            Debug.LogError("TurretUpgradeUI.Instance is null!");
        }
    }
} 