using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    void Awake() 
    {
        // Debug.Log("CurrencyManager Awake called");
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple CurrencyManager instances detected! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        // Debug.Log($"CurrencyManager initialized with {currency} currency");
    }
    
    void Start()
    {
        // Debug.Log($"CurrencyManager Start - Current currency: {currency}");
        //ensure the UI is updated with the initial currency value
        if (UIManager.Instance != null)
        {
            UIManager.Instance.updateCurrencyUI();
        }
        else
        {
            Debug.LogError("UIManager.Instance is null in CurrencyManager Start!");
        }
    }

    //set initial currency to 500 for a more balanced start
    public int currency = 300;

    public void AddCurrency(int amount) 
    {
        int oldCurrency = currency;
        currency += amount;
        // Debug.Log($"Currency ADDED: {amount}, Old: {oldCurrency}, New: {currency}");
        
        //update the UI to reflect the new currency value
        if (UIManager.Instance != null)
        {
            UIManager.Instance.updateCurrencyUI();
        }
        else
        {
            Debug.LogError("UIManager.Instance is null in AddCurrency!");
        }
    }

    public void RemoveCurrency(int amount) 
    {
        int oldCurrency = currency;
        currency -= amount;
        // Debug.Log($"Currency REMOVED: {amount}, Old: {oldCurrency}, New: {currency}");
        
        //update the UI to reflect the new currency value
        if (UIManager.Instance != null)
        {
            UIManager.Instance.updateCurrencyUI();
        }
        else
        {
            Debug.LogError("UIManager.Instance is null in RemoveCurrency!");
        }
    }

    public bool CanAfford(int amount)
    {
        bool result = currency >= amount;
        // Debug.Log($"CanAfford check: Amount: {amount}, Currency: {currency}, Result: {result}");
        return result;
    }
}
