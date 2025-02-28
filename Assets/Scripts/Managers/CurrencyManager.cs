using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public static CurrencyManager Instance;

    void Awake() {
        Instance = this;
    }

    public int currency = 5;

    public void AddCurrency(int amount) {
        currency += amount;
        // Update the UI to reflect the new currency value
        UIManager.Instance.updateCurrencyUI();
    }

    public void RemoveCurrency(int amount) {
        currency -= amount;
        // Update the UI to reflect the new currency value
        UIManager.Instance.updateCurrencyUI();
    }

    public bool CanAfford(int amount)
    {
        return currency >= amount;
    }
}
