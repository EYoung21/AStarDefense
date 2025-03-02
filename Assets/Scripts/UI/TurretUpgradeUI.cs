using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class TurretUpgradeUI : MonoBehaviour
{
    public static TurretUpgradeUI Instance { get; private set; }

    private void Awake()
    {
        Debug.Log("TurretUpgradeUI Awake called");
        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroying duplicate TurretUpgradeUI instance");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log($"TurretUpgradeUI instance set. GameObject active: {gameObject.activeInHierarchy}, Component enabled: {enabled}");
        
        //make sure this GameObject stays active
        gameObject.SetActive(true);
        
        //only deactivate the panel itself
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
        else
        {
            Debug.LogError("upgradePanel reference is missing! Please assign it in the Inspector.");
        }
    }

    private void OnEnable()
    {
        Debug.Log("TurretUpgradeUI OnEnable called");
    }

    private void OnDisable()
    {
        Debug.Log("TurretUpgradeUI OnDisable called");
        //don't clear Instance when disabled
        //if (Instance == this)
        //    Instance = null;
    }

    [System.Serializable]
    public class UpgradeButton
    {
        public Button button;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI costText;
        public Image icon;
    }

    [Header("Upgrade Buttons")]
    [SerializeField] private UpgradeButton frostButton;
    [SerializeField] private UpgradeButton poisonButton;
    [SerializeField] private UpgradeButton splashButton;
    [SerializeField] private UpgradeButton rapidFireButton;
    [SerializeField] private UpgradeButton sniperButton;

    [Header("UI Elements")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private TextMeshProUGUI turretNameText;

    private BaseTurret selectedTurret;
    private TurretUpgrade turretUpgrade;
    private bool justShown = false;

    private void Start()
    {
        Debug.Log("TurretUpgradeUI Start - Checking references:");
        Debug.Log($"upgradePanel reference: {(upgradePanel != null ? "Set" : "Missing")}");
        Debug.Log($"turretNameText reference: {(turretNameText != null ? "Set" : "Missing")}");
        
        //check button references
        Debug.Log($"frostButton reference: {(frostButton?.button != null ? "Set" : "Missing")}");
        Debug.Log($"poisonButton reference: {(poisonButton?.button != null ? "Set" : "Missing")}");
        Debug.Log($"splashButton reference: {(splashButton?.button != null ? "Set" : "Missing")}");
        Debug.Log($"rapidFireButton reference: {(rapidFireButton?.button != null ? "Set" : "Missing")}");
        Debug.Log($"sniperButton reference: {(sniperButton?.button != null ? "Set" : "Missing")}");

        InitializeButtons();
        HideUpgradePanel();
        
        //add a key press handler for testing
        StartCoroutine(TestKeyPressHandler());
    }

    private System.Collections.IEnumerator TestKeyPressHandler()
    {
        while (true)
        {
            //press 1-5 to simulate clicking upgrade buttons
            if (Input.GetKeyDown(KeyCode.Alpha1) && upgradePanel.activeSelf)
            {
                Debug.Log("Test: Simulating Frost button click via key press");
                PurchaseUpgrade(TurretUpgrade.UpgradeType.Frost);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && upgradePanel.activeSelf)
            {
                Debug.Log("Test: Simulating Poison button click via key press");
                PurchaseUpgrade(TurretUpgrade.UpgradeType.Poison);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && upgradePanel.activeSelf)
            {
                Debug.Log("Test: Simulating Splash button click via key press");
                PurchaseUpgrade(TurretUpgrade.UpgradeType.Splash);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) && upgradePanel.activeSelf)
            {
                Debug.Log("Test: Simulating RapidFire button click via key press");
                PurchaseUpgrade(TurretUpgrade.UpgradeType.RapidFire);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5) && upgradePanel.activeSelf)
            {
                Debug.Log("Test: Simulating Sniper button click via key press");
                PurchaseUpgrade(TurretUpgrade.UpgradeType.Sniper);
            }
            
            yield return null;
        }
    }

    private void Update()
    {
        //test key presses for direct upgrades (no button clicks needed)
        if (upgradePanel != null && upgradePanel.activeSelf && turretUpgrade != null)
        {
            //use number keys 1-5 for direct upgrade purchases
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("DIRECT UPGRADE: Frost via keyboard shortcut");
                DirectPurchaseUpgrade(TurretUpgrade.UpgradeType.Frost);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("DIRECT UPGRADE: Poison via keyboard shortcut");
                DirectPurchaseUpgrade(TurretUpgrade.UpgradeType.Poison);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("DIRECT UPGRADE: Splash via keyboard shortcut");
                DirectPurchaseUpgrade(TurretUpgrade.UpgradeType.Splash);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Debug.Log("DIRECT UPGRADE: Rapid Fire via keyboard shortcut");
                DirectPurchaseUpgrade(TurretUpgrade.UpgradeType.RapidFire);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Debug.Log("DIRECT UPGRADE: Sniper via keyboard shortcut");
                DirectPurchaseUpgrade(TurretUpgrade.UpgradeType.Sniper);
            }
        }

        //reset justShown flag if mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            justShown = false;
        }

        //only hide panel when pressing space or clicking outside UI
        //don't hide if we just showed the panel this frame
        if (Input.GetKeyDown(KeyCode.Space) && !justShown)
        {
            Debug.Log("Hiding upgrade panel due to Space key press");
            HideUpgradePanel();
        }
        
        //only check for mouse clicks to hide panel if we're not clicking on UI
        if (Input.GetMouseButtonDown(0) && !justShown)
        {
            //use a more reliable method to check if we're clicking on UI
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            
            //if we didn't hit any UI elements, hide the panel
            if (results.Count == 0)
            {
                Debug.Log("Hiding upgrade panel due to click outside UI");
                HideUpgradePanel();
            }
        }
    }

    private void InitializeButtons()
    {
        Debug.Log("InitializeButtons called");
        
        //initialize frost button
        if (frostButton != null && frostButton.button != null)
        {
            frostButton.button.onClick.RemoveAllListeners();
            frostButton.button.onClick.AddListener(() => {
                Debug.Log("Frost button clicked");
                //stop event propagation
                EventSystem.current.SetSelectedGameObject(null);
                PurchaseUpgrade(TurretUpgrade.UpgradeType.Frost);
            });
        }
        else
        {
            Debug.LogError("Frost button reference is missing!");
        }
        
        //initialize poison button
        if (poisonButton != null && poisonButton.button != null)
        {
            poisonButton.button.onClick.RemoveAllListeners();
            poisonButton.button.onClick.AddListener(() => {
                Debug.Log("Poison button clicked");
                //stop event propagation
                EventSystem.current.SetSelectedGameObject(null);
                PurchaseUpgrade(TurretUpgrade.UpgradeType.Poison);
            });
        }
        else
        {
            Debug.LogError("Poison button reference is missing!");
        }
        
        //initialize splash button
        if (splashButton != null && splashButton.button != null)
        {
            splashButton.button.onClick.RemoveAllListeners();
            splashButton.button.onClick.AddListener(() => {
                Debug.Log("Splash button clicked");
                //stop event propagation
                EventSystem.current.SetSelectedGameObject(null);
                PurchaseUpgrade(TurretUpgrade.UpgradeType.Splash);
            });
        }
        else
        {
            Debug.LogError("Splash button reference is missing!");
        }
        
        //initialize rapid fire button
        if (rapidFireButton != null && rapidFireButton.button != null)
        {
            rapidFireButton.button.onClick.RemoveAllListeners();
            rapidFireButton.button.onClick.AddListener(() => {
                Debug.Log("Rapid Fire button clicked");
                //stop event propagation
                EventSystem.current.SetSelectedGameObject(null);
                PurchaseUpgrade(TurretUpgrade.UpgradeType.RapidFire);
            });
        }
        else
        {
            Debug.LogError("Rapid Fire button reference is missing!");
        }
        
        //initialize sniper button
        if (sniperButton != null && sniperButton.button != null)
        {
            sniperButton.button.onClick.RemoveAllListeners();
            sniperButton.button.onClick.AddListener(() => {
                Debug.Log("Sniper button clicked");
                //stop event propagation
                EventSystem.current.SetSelectedGameObject(null);
                PurchaseUpgrade(TurretUpgrade.UpgradeType.Sniper);
            });
        }
        else
        {
            Debug.LogError("Sniper button reference is missing!");
        }
        
        Debug.Log("All buttons initialized");
    }

    public void ShowUpgradePanel(BaseTurret turret)
    {
        Debug.Log($"Attempting to show upgrade panel for turret: {turret.name}");
        selectedTurret = turret;
        
        //try to get the TurretUpgrade component directly from the turret
        turretUpgrade = turret.GetComponent<TurretUpgrade>();
        
        //if not found, try to find it in children
        if (turretUpgrade == null)
        {
            Debug.Log("TurretUpgrade not found directly on turret, checking children...");
            turretUpgrade = turret.GetComponentInChildren<TurretUpgrade>();
        }
        
        //if still not found, try to add it
        if (turretUpgrade == null)
        {
            Debug.LogWarning($"No TurretUpgrade component found on turret: {turret.name}. Adding one now.");
            turretUpgrade = turret.gameObject.AddComponent<TurretUpgrade>();
            
            //wait for the next frame to ensure the component is properly initialized
            StartCoroutine(DelayedShowUpgradePanel());
            return;
        }

        CompleteShowUpgradePanel();
    }
    
    private IEnumerator DelayedShowUpgradePanel()
    {
        //wait for the TurretUpgrade component to initialize
        yield return new WaitForEndOfFrame();
        CompleteShowUpgradePanel();
    }
    
    private void CompleteShowUpgradePanel()
    {
        if (upgradePanel == null)
        {
            Debug.LogError("upgradePanel reference is missing in TurretUpgradeUI!");
            return;
        }

        upgradePanel.SetActive(true);
        justShown = true;
        Debug.Log("Set justShown to true when showing upgrade panel");
        
        //set the turret name
        if (turretNameText != null && selectedTurret != null)
        {
            turretNameText.text = selectedTurret.UnitName;
            Debug.Log($"Set turret name text to: {selectedTurret.UnitName}");
        }

        Debug.Log($"Upgrade panel shown. Panel active state: {upgradePanel.activeSelf}");
        
        UpdateUI();
        
        //display a message to the user about keyboard shortcuts
        Debug.Log("IMPORTANT: You can use number keys 1-5 to directly purchase upgrades without clicking buttons");
    }

    private void UpdateUI()
    {
        if (turretUpgrade == null) return;

        UpdateUpgradeButton(frostButton, TurretUpgrade.UpgradeType.Frost);
        UpdateUpgradeButton(poisonButton, TurretUpgrade.UpgradeType.Poison);
        UpdateUpgradeButton(splashButton, TurretUpgrade.UpgradeType.Splash);
        UpdateUpgradeButton(rapidFireButton, TurretUpgrade.UpgradeType.RapidFire);
        UpdateUpgradeButton(sniperButton, TurretUpgrade.UpgradeType.Sniper);
    }

    private void UpdateUpgradeButton(UpgradeButton button, TurretUpgrade.UpgradeType type)
    {
        if (button == null || button.button == null) return;

        int level = turretUpgrade.GetCurrentLevel(type);
        int cost = turretUpgrade.GetUpgradeCost(type);
        bool canAfford = CurrencyManager.Instance.currency >= cost;
        bool canUpgrade = turretUpgrade.CanUpgrade(type);

        Debug.Log($"Updating button for {type}: Level={level}, Cost={cost}, CanAfford={canAfford}, CanUpgrade={canUpgrade}, Currency={CurrencyManager.Instance.currency}");

        string upgradeName = turretUpgrade.GetUpgradeName(type);
        string description = turretUpgrade.GetUpgradeDescription(type);
        
        //update the level text with the proper upgrade name
        if (button.levelText != null)
        {
            button.levelText.text = $"{upgradeName}\nLevel {level}/3\n{description}";
        }
        
        //update the cost text
        if (button.costText != null)
        {
            button.costText.text = level >= 3 ? "MAX" : $"Cost: {cost}";
        }
        
        //make sure the button is interactable and covers the full area
        button.button.interactable = canAfford && canUpgrade;
        Debug.Log($"Button {type} interactable set to: {button.button.interactable}");
        
        //make sure the button's image component fills the entire area
        if (button.button.image != null)
        {
            button.button.image.type = Image.Type.Sliced;
            button.button.image.fillCenter = true;
        }
        
        //make sure the button has proper navigation
        Navigation nav = new Navigation();
        nav.mode = Navigation.Mode.Automatic;
        button.button.navigation = nav;
        
        //make sure the button has proper colors for different states
        ColorBlock colors = button.button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
        colors.pressedColor = new Color(0.8f, 0.8f, 0.8f);
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        colors.fadeDuration = 0.1f;  //make the transitions snappier
        button.button.colors = colors;
    }

    private void PurchaseUpgrade(TurretUpgrade.UpgradeType type)
    {
        Debug.Log($"PurchaseUpgrade called for {type}");
        
        //set justShown to true immediately to prevent panel from closing
        justShown = true;
        
        if (turretUpgrade == null)
        {
            Debug.LogError("turretUpgrade is null when trying to purchase upgrade");
            
            //try to recover the reference
            if (selectedTurret != null)
            {
                Debug.Log($"Attempting to recover turretUpgrade reference from {selectedTurret.name}");
                turretUpgrade = selectedTurret.GetComponent<TurretUpgrade>();
                if (turretUpgrade == null)
                {
                    turretUpgrade = selectedTurret.GetComponentInChildren<TurretUpgrade>();
                }
                
                if (turretUpgrade == null)
                {
                    Debug.LogError("Failed to recover turretUpgrade reference!");
                    return;
                }
                else
                {
                    Debug.Log("Successfully recovered turretUpgrade reference.");
                }
            }
            else
            {
                Debug.LogError("selectedTurret is also null!");
                return;
            }
        }

        int cost = turretUpgrade.GetUpgradeCost(type);
        int currentCurrency = CurrencyManager.Instance.currency;
        Debug.Log($"Upgrade cost: {cost}, Available currency: {currentCurrency}");
        
        if (currentCurrency >= cost)
        {
            Debug.Log($"Purchasing upgrade: {type} for {cost} currency");
            
            //store the current level for verification
            int previousLevel = turretUpgrade.GetCurrentLevel(type);
            Debug.Log($"Current upgrade level before purchase: {previousLevel}");
            
            //apply the upgrade BEFORE removing currency to ensure it works
            bool upgradeSuccess = turretUpgrade.ApplyUpgrade(type);
            Debug.Log($"ApplyUpgrade result: {upgradeSuccess}");
            
            //verify the upgrade was applied
            int newLevel = turretUpgrade.GetCurrentLevel(type);
            Debug.Log($"New upgrade level after purchase: {newLevel}");
            
            if (newLevel > previousLevel && upgradeSuccess)
            {
                Debug.Log($"Upgrade level increased from {previousLevel} to {newLevel}.");
                
                //remove currency AFTER successful upgrade
                CurrencyManager.Instance.RemoveCurrency(cost);
                Debug.Log($"Currency reduced from {currentCurrency} to {CurrencyManager.Instance.currency}");
                
                //update the UI to reflect changes
                UpdateUI();
                
                //keep the panel open after purchase
                justShown = true;
                Debug.Log("Set justShown to true to keep panel open after purchase");
                
                //add a small delay before allowing the panel to close
                StartCoroutine(ResetJustShownAfterDelay(0.5f));
            }
            else
            {
                Debug.LogError($"Upgrade failed! Level did not increase from {previousLevel}");
            }
        }
        else
        {
            Debug.Log("Not enough currency to purchase upgrade");
        }
    }

    private IEnumerator ResetJustShownAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        justShown = false;
    }

    public bool IsUpgradePanelActive()
    {
        return upgradePanel != null && upgradePanel.activeSelf;
    }

    //direct purchase method that bypasses button clicks
    private void DirectPurchaseUpgrade(TurretUpgrade.UpgradeType type)
    {
        if (turretUpgrade == null)
        {
            Debug.LogError("turretUpgrade is null when trying direct purchase");
            return;
        }

        int cost = turretUpgrade.GetUpgradeCost(type);
        int currentCurrency = CurrencyManager.Instance.currency;
        Debug.Log($"Direct upgrade cost: {cost}, Available currency: {currentCurrency}");
        
        if (currentCurrency >= cost)
        {
            Debug.Log($"Direct purchasing upgrade: {type} for {cost} currency");
            
            //store the current level for verification
            int previousLevel = turretUpgrade.GetCurrentLevel(type);
            Debug.Log($"Current upgrade level before direct purchase: {previousLevel}");
            
            //apply the upgrade
            bool upgradeSuccess = turretUpgrade.ApplyUpgrade(type);
            Debug.Log($"Direct ApplyUpgrade result: {upgradeSuccess}");
            
            //verify the upgrade was applied
            int newLevel = turretUpgrade.GetCurrentLevel(type);
            Debug.Log($"New upgrade level after direct purchase: {newLevel}");
            
            if (newLevel > previousLevel && upgradeSuccess)
            {
                Debug.Log($"Direct upgrade level increased from {previousLevel} to {newLevel}.");
                
                //remove currency
                CurrencyManager.Instance.RemoveCurrency(cost);
                Debug.Log($"Currency reduced from {currentCurrency} to {CurrencyManager.Instance.currency}");
                
                //update the UI to reflect changes
                UpdateUI();
            }
            else
            {
                Debug.LogError($"Direct upgrade failed! Level did not increase from {previousLevel}");
            }
        }
        else
        {
            Debug.Log("Not enough currency for direct upgrade");
        }
    }

    public void HideUpgradePanel()
    {
        Debug.Log("HideUpgradePanel called");
        
        //don't hide if justShown is true
        if (justShown)
        {
            Debug.Log("Panel was just shown, not hiding it");
            return;
        }
        
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
            Debug.Log("Upgrade panel hidden");
            
            //clear references when actually hiding
            selectedTurret = null;
            turretUpgrade = null;
        }
        else
        {
            Debug.LogError("Cannot hide upgrade panel: upgradePanel reference is null!");
        }
    }
} 