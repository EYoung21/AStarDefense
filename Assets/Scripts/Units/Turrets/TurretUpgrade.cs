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
        turret = GetComponent<BaseTurret>();
        InitializeUpgradeStats();
    }

    void InitializeUpgradeStats()
    {
        upgradeStats = new Dictionary<UpgradeType, UpgradeLevel[]>();

        // Initialize Frost upgrades
        upgradeStats[UpgradeType.Frost] = new UpgradeLevel[] {
            new UpgradeLevel {
                upgradeName = "Frost Turret",
                description = "Slows enemies by 20%",
                cost = 100,
                slowEffect = 0.2f
            },
            new UpgradeLevel {
                upgradeName = "Deep Freeze",
                description = "Slows enemies by 35%",
                cost = 200,
                slowEffect = 0.35f
            },
            new UpgradeLevel {
                upgradeName = "Absolute Zero",
                description = "Slows enemies by 50%",
                cost = 300,
                slowEffect = 0.5f
            }
        };

        // Initialize Poison upgrades
        upgradeStats[UpgradeType.Poison] = new UpgradeLevel[] {
            new UpgradeLevel {
                upgradeName = "Poison Turret",
                description = "Deals 2 poison damage per second",
                cost = 100,
                poisonDamage = 2f
            },
            new UpgradeLevel {
                upgradeName = "Potent Poison",
                description = "Deals 4 poison damage per second",
                cost = 200,
                poisonDamage = 4f
            },
            new UpgradeLevel {
                upgradeName = "Deadly Venom",
                description = "Deals 6 poison damage per second",
                cost = 300,
                poisonDamage = 6f
            }
        };

        // Initialize Splash upgrades
        upgradeStats[UpgradeType.Splash] = new UpgradeLevel[] {
            new UpgradeLevel {
                upgradeName = "Splash Turret",
                description = "1 unit splash radius, 50% splash damage",
                cost = 150,
                splashRadius = 1f,
                splashDamageMultiplier = 0.5f
            },
            new UpgradeLevel {
                upgradeName = "Wide Splash",
                description = "1.5 unit splash radius, 60% splash damage",
                cost = 250,
                splashRadius = 1.5f,
                splashDamageMultiplier = 0.6f
            },
            new UpgradeLevel {
                upgradeName = "Massive Splash",
                description = "2 unit splash radius, 75% splash damage",
                cost = 350,
                splashRadius = 2f,
                splashDamageMultiplier = 0.75f
            }
        };

        // Initialize RapidFire upgrades
        upgradeStats[UpgradeType.RapidFire] = new UpgradeLevel[] {
            new UpgradeLevel {
                upgradeName = "Rapid Fire",
                description = "20% faster attack speed",
                cost = 100,
                attackSpeedMultiplier = 1.2f
            },
            new UpgradeLevel {
                upgradeName = "Quick Shot",
                description = "40% faster attack speed",
                cost = 200,
                attackSpeedMultiplier = 1.4f
            },
            new UpgradeLevel {
                upgradeName = "Lightning Shot",
                description = "60% faster attack speed",
                cost = 300,
                attackSpeedMultiplier = 1.6f
            }
        };

        // Initialize Sniper upgrades
        upgradeStats[UpgradeType.Sniper] = new UpgradeLevel[] {
            new UpgradeLevel {
                upgradeName = "Sniper Turret",
                description = "25% more damage, 20% more range",
                cost = 150,
                damageMultiplier = 1.25f,
                rangeMultiplier = 1.2f
            },
            new UpgradeLevel {
                upgradeName = "Long Shot",
                description = "50% more damage, 40% more range",
                cost = 250,
                damageMultiplier = 1.5f,
                rangeMultiplier = 1.4f
            },
            new UpgradeLevel {
                upgradeName = "Sniper Elite",
                description = "100% more damage, 60% more range",
                cost = 350,
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

    public void ApplyUpgrade(UpgradeType type)
    {
        if (!upgradeLevels.ContainsKey(type))
            upgradeLevels[type] = 0;

        if (upgradeLevels[type] < 3)
        {
            upgradeLevels[type]++;
            UpdateTurretStats();
        }
    }

    private void UpdateTurretStats()
    {
        if (turret == null) return;

        float finalDamage = 1f;
        float finalRange = 1f;
        float finalAttackSpeed = 1f;
        float totalSlowEffect = 0f;
        float totalPoisonDamage = 0f;
        float totalSplashRadius = 0f;
        float totalSplashDamage = 0f;
        float totalLifeLeech = 0f;

        foreach (var upgrade in upgradeLevels)
        {
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

        turret.UpdateStats(finalDamage, finalRange, finalAttackSpeed);
        turret.UpdateEffects(totalSlowEffect, totalPoisonDamage, totalSplashRadius, totalSplashDamage, totalLifeLeech);
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
} 