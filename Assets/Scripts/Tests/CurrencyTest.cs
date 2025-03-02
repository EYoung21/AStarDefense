using UnityEngine;
using UnityEngine.UI;

public class CurrencyTest : MonoBehaviour
{
    [SerializeField] private Button addCurrencyButton;
    [SerializeField] private Button removeCurrencyButton;
    [SerializeField] private int testAmount = 100;

    void Start()
    {
        Debug.Log("CurrencyTest Start - Setting up test buttons");
        
        if (addCurrencyButton != null)
        {
            addCurrencyButton.onClick.AddListener(() => {
                Debug.Log($"Test: Adding {testAmount} currency");
                if (CurrencyManager.Instance != null)
                {
                    CurrencyManager.Instance.AddCurrency(testAmount);
                    Debug.Log($"Test: New currency amount: {CurrencyManager.Instance.currency}");
                }
                else
                {
                    Debug.LogError("CurrencyManager.Instance is null in CurrencyTest!");
                }
            });
        }
        
        if (removeCurrencyButton != null)
        {
            removeCurrencyButton.onClick.AddListener(() => {
                Debug.Log($"Test: Removing {testAmount} currency");
                if (CurrencyManager.Instance != null)
                {
                    CurrencyManager.Instance.RemoveCurrency(testAmount);
                    Debug.Log($"Test: New currency amount: {CurrencyManager.Instance.currency}");
                }
                else
                {
                    Debug.LogError("CurrencyManager.Instance is null in CurrencyTest!");
                }
            });
        }
    }
    
    //add a key press test for easier debugging
    void Update()
    {
        //press 'A' to add currency
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log($"Test: Adding {testAmount} currency via key press");
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddCurrency(testAmount);
                Debug.Log($"Test: New currency amount: {CurrencyManager.Instance.currency}");
            }
        }
        
        //press 'S' to subtract currency
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log($"Test: Removing {testAmount} currency via key press");
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.RemoveCurrency(testAmount);
                Debug.Log($"Test: New currency amount: {CurrencyManager.Instance.currency}");
            }
        }
        
        //press 'P' to print current currency
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (CurrencyManager.Instance != null)
            {
                Debug.Log($"Test: Current currency amount: {CurrencyManager.Instance.currency}");
            }
            else
            {
                Debug.LogError("CurrencyManager.Instance is null in CurrencyTest!");
            }
        }
    }
} 