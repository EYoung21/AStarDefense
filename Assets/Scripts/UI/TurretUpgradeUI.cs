using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

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
        
        // Make sure this GameObject stays active
        gameObject.SetActive(true);
        
        // Only deactivate the panel itself
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
        // Don't clear Instance when disabled
        // if (Instance == this)
        //     Instance = null;
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
        
        // Check button references
        Debug.Log($"frostButton reference: {(frostButton?.button != null ? "Set" : "Missing")}");
        Debug.Log($"poisonButton reference: {(poisonButton?.button != null ? "Set" : "Missing")}");
        Debug.Log($"splashButton reference: {(splashButton?.button != null ? "Set" : "Missing")}");
        Debug.Log($"rapidFireButton reference: {(rapidFireButton?.button != null ? "Set" : "Missing")}");
        Debug.Log($"sniperButton reference: {(sniperButton?.button != null ? "Set" : "Missing")}");

        InitializeButtons();
        HideUpgradePanel();
    }

    private void Update()
    {
        // Reset justShown flag if mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            justShown = false;
        }

        // Hide panel when pressing space or clicking empty tile
        // Don't hide if we just showed the panel this frame
        if ((Input.GetKeyDown(KeyCode.Space) || 
            (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())) && 
            !justShown)
        {
            HideUpgradePanel();
        }
    }

    private void InitializeButtons()
    {
        frostButton.button.onClick.AddListener(() => PurchaseUpgrade(TurretUpgrade.UpgradeType.Frost));
        poisonButton.button.onClick.AddListener(() => PurchaseUpgrade(TurretUpgrade.UpgradeType.Poison));
        splashButton.button.onClick.AddListener(() => PurchaseUpgrade(TurretUpgrade.UpgradeType.Splash));
        rapidFireButton.button.onClick.AddListener(() => PurchaseUpgrade(TurretUpgrade.UpgradeType.RapidFire));
        sniperButton.button.onClick.AddListener(() => PurchaseUpgrade(TurretUpgrade.UpgradeType.Sniper));
    }

    public void ShowUpgradePanel(BaseTurret turret)
    {
        Debug.Log($"Attempting to show upgrade panel for turret: {turret.name}");
        selectedTurret = turret;
        turretUpgrade = turret.GetComponent<TurretUpgrade>();
        
        if (turretUpgrade == null) {
            Debug.LogError($"No TurretUpgrade component found on turret: {turret.name}");
            return;
        }

        if (upgradePanel == null) {
            Debug.LogError("upgradePanel reference is missing in TurretUpgradeUI!");
            return;
        }

        upgradePanel.SetActive(true);
        justShown = true;  // Set the flag when showing the panel
        
        // Set the turret name
        if (turretNameText != null) {
            turretNameText.text = turret.UnitName;
        }

        Debug.Log($"Upgrade panel shown. Panel active state: {upgradePanel.activeSelf}");
        UpdateUI();
    }

    public void HideUpgradePanel()
    {
        Debug.Log("Hiding upgrade panel");
        if (upgradePanel != null) {
            upgradePanel.SetActive(false);
            justShown = false;  // Reset the flag when hiding
        } else {
            Debug.LogError("upgradePanel reference is missing!");
        }
        selectedTurret = null;
        turretUpgrade = null;
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

        string upgradeName = turretUpgrade.GetUpgradeName(type);
        string description = turretUpgrade.GetUpgradeDescription(type);
        
        // Update the level text with the proper upgrade name
        if (button.levelText != null)
        {
            button.levelText.text = $"{upgradeName}\nLevel {level}/3\n{description}";
        }
        
        // Update the cost text
        if (button.costText != null)
        {
            button.costText.text = level >= 3 ? "MAX" : $"Cost: {cost}";
        }
        
        // Make sure the button is interactable and covers the full area
        button.button.interactable = canAfford && canUpgrade;
        
        // Make sure the button's image component fills the entire area
        if (button.button.image != null)
        {
            button.button.image.type = Image.Type.Sliced;
            button.button.image.fillCenter = true;
        }
        
        // Make sure the button has proper navigation
        Navigation nav = new Navigation();
        nav.mode = Navigation.Mode.Automatic;
        button.button.navigation = nav;
        
        // Make sure the button has proper colors for different states
        ColorBlock colors = button.button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
        colors.pressedColor = new Color(0.8f, 0.8f, 0.8f);
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        colors.fadeDuration = 0.1f;  // Make the transitions snappier
        button.button.colors = colors;
    }

    private void PurchaseUpgrade(TurretUpgrade.UpgradeType type)
    {
        if (turretUpgrade == null) return;

        int cost = turretUpgrade.GetUpgradeCost(type);
        if (CurrencyManager.Instance.currency >= cost)
        {
            CurrencyManager.Instance.RemoveCurrency(cost);
            turretUpgrade.ApplyUpgrade(type);
            UpdateUI();
        }
    }

    public bool IsUpgradePanelActive()
    {
        return upgradePanel != null && upgradePanel.activeSelf;
    }
} 