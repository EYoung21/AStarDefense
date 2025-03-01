using UnityEngine;
using System.Collections.Generic;

// Temporarily rename to help identify the conflict
public class TurretUpgrade : MonoBehaviour
{
    [System.Serializable]
    public class UpgradeLevel
    {
        public string upgradeName;
        public string description;
        public int cost;
        public float damageMultiplier = 1f;
        public float rangeMultiplier = 1f;
        public float attackSpeedMultiplier = 1f;
        public float slowEffect = 0f;
        public float poisonDamage = 0f;
        public float splashRadius = 0f;
        public float splashDamageMultiplier = 0f;
        public float lifeLeechAmount = 0f;
    }

    public enum UpgradeType
    {
        Frost,
        Poison,
        Splash,
        RapidFire,
        Sniper
    }

    private BaseTurret turret;
    private Dictionary<UpgradeType, int> upgradeLevels = new Dictionary<UpgradeType, int>();
    private Dictionary<UpgradeType, UpgradeLevel[]> upgradeStats;

    void Awake()
    {
        Debug.Log($"TurretUpgrade.Awake called on {gameObject.name}");
        turret = GetComponent<BaseTurret>();
        
        if (turret != null)
        {
            Debug.Log($"Found BaseTurret component: {turret.name}");
        }
        else
        {
            Debug.LogError($"Failed to find BaseTurret component on {gameObject.name}! Upgrades will not work.");
            // Try to find the BaseTurret component in parent or children
            turret = GetComponentInParent<BaseTurret>();
            if (turret != null)
            {
                Debug.Log($"Found BaseTurret component in parent: {turret.name}");
            }
            else
            {
                turret = GetComponentInChildren<BaseTurret>();
                if (turret != null)
                {
                    Debug.Log($"Found BaseTurret component in children: {turret.name}");
                }
            }
        }
        
        InitializeUpgradeStats();
    }

    void OnEnable()
    {
        // Double-check that we have a valid turret reference
        if (turret == null)
        {
            turret = GetComponent<BaseTurret>();
            if (turret == null)
            {
                turret = GetComponentInParent<BaseTurret>();
                if (turret == null)
                {
                    turret = GetComponentInChildren<BaseTurret>();
                }
            }
            
            if (turret != null)
            {
                Debug.Log($"Restored turret reference in OnEnable: {turret.name}");
            }
            else
            {
                Debug.LogError("Failed to restore turret reference in OnEnable!");
            }
        }
    }

    void Update()
    {
        // Press 'U' to print upgrade levels
        if (Input.GetKeyDown(KeyCode.U))
        {
            PrintUpgradeLevels();
        }
    }

    void InitializeUpgradeStats()
    {
        upgradeStats = new Dictionary<UpgradeType, UpgradeLevel[]>();

        // Initialize Frost upgrades
        upgradeStats[UpgradeType.Frost] = new UpgradeLevel[] {
            new UpgradeLevel {
                upgradeName = "Frost Turret",
                description = "Slows enemies by 20%",
                cost = 75,
                slowEffect = 0.2f
            },
            new UpgradeLevel {
                upgradeName = "Deep Freeze",
                description = "Slows enemies by 35%",
                cost = 150,
                slowEffect = 0.35f
            },
            new UpgradeLevel {
                upgradeName = "Absolute Zero",
                description = "Slows enemies by 50%",
                cost = 250,
                slowEffect = 0.5f
            }
        };

        // Initialize Poison upgrades
        upgradeStats[UpgradeType.Poison] = new UpgradeLevel[] {
            new UpgradeLevel {
                upgradeName = "Poison Turret",
                description = "Deals 2 poison damage per second",
                cost = 75,
                poisonDamage = 2f
            },
            new UpgradeLevel {
                upgradeName = "Potent Poison",
                description = "Deals 4 poison damage per second",
                cost = 150,
                poisonDamage = 4f
            },
            new UpgradeLevel {
                upgradeName = "Deadly Venom",
                description = "Deals 6 poison damage per second",
                cost = 250,
                poisonDamage = 6f
            }
        };

        // Initialize Splash upgrades
        upgradeStats[UpgradeType.Splash] = new UpgradeLevel[] {
            new UpgradeLevel {
                upgradeName = "Splash Turret",
                description = "1 unit splash radius, 50% splash damage",
                cost = 100,
                splashRadius = 1f,
                splashDamageMultiplier = 0.5f
            },
            new UpgradeLevel {
                upgradeName = "Wide Splash",
                description = "1.5 unit splash radius, 60% splash damage",
                cost = 200,
                splashRadius = 1.5f,
                splashDamageMultiplier = 0.6f
            },
            new UpgradeLevel {
                upgradeName = "Massive Splash",
                description = "2 unit splash radius, 75% splash damage",
                cost = 300,
                splashRadius = 2f,
                splashDamageMultiplier = 0.75f
            }
        };

        // Initialize RapidFire upgrades
        upgradeStats[UpgradeType.RapidFire] = new UpgradeLevel[] {
            new UpgradeLevel {
                upgradeName = "Rapid Fire",
                description = "20% faster attack speed",
                cost = 75,
                attackSpeedMultiplier = 1.2f
            },
            new UpgradeLevel {
                upgradeName = "Quick Shot",
                description = "40% faster attack speed",
                cost = 150,
                attackSpeedMultiplier = 1.4f
            },
            new UpgradeLevel {
                upgradeName = "Lightning Shot",
                description = "60% faster attack speed",
                cost = 250,
                attackSpeedMultiplier = 1.6f
            }
        };

        // Initialize Sniper upgrades
        upgradeStats[UpgradeType.Sniper] = new UpgradeLevel[] {
            new UpgradeLevel {
                upgradeName = "Sniper Turret",
                description = "25% more damage, 20% more range",
                cost = 100,
                damageMultiplier = 1.25f,
                rangeMultiplier = 1.2f
            },
            new UpgradeLevel {
                upgradeName = "Long Shot",
                description = "50% more damage, 40% more range",
                cost = 200,
                damageMultiplier = 1.5f,
                rangeMultiplier = 1.4f
            },
            new UpgradeLevel {
                upgradeName = "Sniper Elite",
                description = "100% more damage, 60% more range",
                cost = 300,
                damageMultiplier = 2f,
                rangeMultiplier = 1.6f
            }
        };
    }

    public bool CanUpgrade(UpgradeType type)
    {
        if (!upgradeLevels.ContainsKey(type))
            return true;
        return upgradeLevels[type] < 3;
    }

    public int GetUpgradeCost(UpgradeType type)
    {
        if (!upgradeLevels.ContainsKey(type))
            return upgradeStats[type][0].cost;
        if (upgradeLevels[type] >= 3)
            return -1;
        return upgradeStats[type][upgradeLevels[type]].cost;
    }

    public bool ApplyUpgrade(UpgradeType type)
    {
        Debug.Log($"TurretUpgrade.ApplyUpgrade called for {type}");
        
        // Double-check that we have a valid turret reference
        if (turret == null)
        {
            turret = GetComponent<BaseTurret>();
            if (turret == null)
            {
                turret = GetComponentInParent<BaseTurret>();
                if (turret == null)
                {
                    turret = GetComponentInChildren<BaseTurret>();
                }
            }
            
            if (turret == null)
            {
                Debug.LogError("Turret reference is null in ApplyUpgrade! Cannot apply upgrade.");
                return false;
            }
        }
        
        if (!upgradeLevels.ContainsKey(type))
        {
            Debug.Log($"First upgrade for {type}, initializing to level 0");
            upgradeLevels[type] = 0;
        }

        if (upgradeLevels[type] < 3)
        {
            int oldLevel = upgradeLevels[type];
            upgradeLevels[type]++;
            Debug.Log($"Upgraded {type} from level {oldLevel} to level {upgradeLevels[type]}");
            
            // Log the turret reference
            if (turret != null)
            {
                Debug.Log($"Turret reference exists: {turret.name}");
            }
            else
            {
                Debug.LogError("Turret reference is null! Cannot apply stats.");
                return false; // Return early if turret is null
            }
            
            UpdateTurretStats();
            return true;
        }
        else
        {
            Debug.Log($"{type} is already at max level (3)");
            return false;
        }
    }

    private void UpdateTurretStats()
    {
        Debug.Log("UpdateTurretStats called");
        
        if (turret == null)
        {
            Debug.LogError("Cannot update turret stats: turret reference is null!");
            return;
        }

        float finalDamage = 1f;
        float finalRange = 1f;
        float finalAttackSpeed = 1f;
        float totalSlowEffect = 0f;
        float totalPoisonDamage = 0f;
        float totalSplashRadius = 0f;
        float totalSplashDamage = 0f;
        float totalLifeLeech = 0f;

        Debug.Log($"Applying effects from {upgradeLevels.Count} upgrade types");
        
        foreach (var upgrade in upgradeLevels)
        {
            Debug.Log($"Processing upgrade: {upgrade.Key}, Level: {upgrade.Value}");
            
            if (upgrade.Value <= 0 || upgrade.Value > 3)
            {
                Debug.LogError($"Invalid upgrade level: {upgrade.Value} for {upgrade.Key}");
                continue;
            }
            
            var level = upgradeStats[upgrade.Key][upgrade.Value - 1];
            finalDamage *= level.damageMultiplier;
            finalRange *= level.rangeMultiplier;
            finalAttackSpeed *= level.attackSpeedMultiplier;
            
            totalSlowEffect = Mathf.Max(totalSlowEffect, level.slowEffect);
            totalPoisonDamage += level.poisonDamage;
            totalSplashRadius = Mathf.Max(totalSplashRadius, level.splashRadius);
            totalSplashDamage = Mathf.Max(totalSplashDamage, level.splashDamageMultiplier);
            totalLifeLeech += level.lifeLeechAmount;
        }

        Debug.Log($"Final multipliers - Damage: {finalDamage}, Range: {finalRange}, Attack Speed: {finalAttackSpeed}");
        Debug.Log($"Final effects - Slow: {totalSlowEffect}, Poison: {totalPoisonDamage}, Splash Radius: {totalSplashRadius}, Splash Damage: {totalSplashDamage}, Life Leech: {totalLifeLeech}");
        
        turret.UpdateStats(finalDamage, finalRange, finalAttackSpeed);
        turret.UpdateEffects(totalSlowEffect, totalPoisonDamage, totalSplashRadius, totalSplashDamage, totalLifeLeech);
        
        Debug.Log("Turret stats updated successfully");
    }

    public string GetUpgradeDescription(UpgradeType type)
    {
        if (!upgradeLevels.ContainsKey(type))
            return upgradeStats[type][0].description;
        if (upgradeLevels[type] >= 3)
            return "Fully Upgraded";
        return upgradeStats[type][upgradeLevels[type]].description;
    }

    public string GetUpgradeName(UpgradeType type)
    {
        if (!upgradeLevels.ContainsKey(type))
            return upgradeStats[type][0].upgradeName;
        if (upgradeLevels[type] >= 3)
            return upgradeStats[type][2].upgradeName;
        return upgradeStats[type][upgradeLevels[type]].upgradeName;
    }

    public int GetCurrentLevel(UpgradeType type)
    {
        if (!upgradeLevels.ContainsKey(type))
            return 0;
        return upgradeLevels[type];
    }

    public void PrintUpgradeLevels()
    {
        Debug.Log($"=== UPGRADE LEVELS FOR {(turret != null ? turret.name : "UNKNOWN TURRET")} ===");
        
        if (upgradeLevels.Count == 0)
        {
            Debug.Log("No upgrades applied yet.");
            return;
        }
        
        foreach (var upgrade in upgradeLevels)
        {
            Debug.Log($"Upgrade: {upgrade.Key}, Level: {upgrade.Value}");
            
            if (upgrade.Value > 0 && upgrade.Value <= 3)
            {
                var level = upgradeStats[upgrade.Key][upgrade.Value - 1];
                Debug.Log($"  Effects: Damage x{level.damageMultiplier}, Range x{level.rangeMultiplier}, Attack Speed x{level.attackSpeedMultiplier}");
                Debug.Log($"  Special: Slow {level.slowEffect}, Poison {level.poisonDamage}, Splash {level.splashRadius}");
            }
        }
        
        // Check if turret reference is valid
        if (turret != null)
        {
            Debug.Log($"Turret reference is valid: {turret.name}");
        }
        else
        {
            Debug.LogError("Turret reference is null!");
        }
    }
} 