using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurretUpgradeUI : MonoBehaviour
{
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
    [SerializeField] private TextMeshProUGUI goldText;

    private BaseTurret selectedTurret;
    private TurretUpgrade turretUpgrade;

    private void Start()
    {
        InitializeButtons();
        HideUpgradePanel();
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
        selectedTurret = turret;
        turretUpgrade = turret.GetComponent<TurretUpgrade>();
        
        if (turretUpgrade == null) return;

        upgradePanel.SetActive(true);
        UpdateUI();
    }

    public void HideUpgradePanel()
    {
        upgradePanel.SetActive(false);
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

        // Update currency display
        goldText.text = $"Currency: {CurrencyManager.Instance.currency}";
    }

    private void UpdateUpgradeButton(UpgradeButton button, TurretUpgrade.UpgradeType type)
    {
        int level = turretUpgrade.GetCurrentLevel(type);
        int cost = turretUpgrade.GetUpgradeCost(type);
        bool canAfford = CurrencyManager.Instance.currency >= cost;
        bool canUpgrade = turretUpgrade.CanUpgrade(type);

        button.levelText.text = $"Level {level}/3";
        button.costText.text = level >= 3 ? "MAX" : $"{cost}";
        button.button.interactable = canAfford && canUpgrade;
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
} 