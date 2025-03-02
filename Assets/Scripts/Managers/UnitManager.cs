using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    
    private List<ScriptableUnit> _units;

    public BaseUnit SelectedUnit;

    public int localNumberOfEnemiesToSpawn;

    public int enemyCount;
    
    private bool isWaveInProgress = false;
    
    // Enemy spawn weights by round
    [System.Serializable]
    public class EnemySpawnWeight
    {
        public string enemyName;
        public AnimationCurve spawnWeightByRound;
    }
    
    [Header("Enemy Spawn Settings")]
    public EnemySpawnWeight[] enemySpawnWeights;
    public int firstTitanRound = 5;
    
    // Track special enemy spawns
    private bool hasTitanSpawned = false;

    void Awake() {
        Instance = this;
        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
        
        // Initialize default spawn weights if not set in inspector
        if (enemySpawnWeights == null || enemySpawnWeights.Length == 0)
        {
            InitializeDefaultSpawnWeights();
        }
    }
    
    private void InitializeDefaultSpawnWeights()
    {
        enemySpawnWeights = new EnemySpawnWeight[4];
        
        // Default spawn weights for TestEnemy1 (original enemy)
        enemySpawnWeights[0] = new EnemySpawnWeight
        {
            enemyName = "BlueStar",
            spawnWeightByRound = new AnimationCurve(
                new Keyframe(1, 100),  // 100% in round 1
                new Keyframe(3, 50),   // 50% by round 3
                new Keyframe(6, 20),   // 20% by round 6
                new Keyframe(10, 10)   // 10% by round 10+
            )
        };
        
        // Default spawn weights for SpeedEnemy
        enemySpawnWeights[1] = new EnemySpawnWeight
        {
            enemyName = "SpeedEnemy",
            spawnWeightByRound = new AnimationCurve(
                new Keyframe(1, 0),    // 0% in round 1
                new Keyframe(2, 20),   // 20% in round 2
                new Keyframe(5, 30),   // 30% by round 5
                new Keyframe(10, 20)   // 20% by round 10+
            )
        };
        
        // Default spawn weights for TankEnemy
        enemySpawnWeights[2] = new EnemySpawnWeight
        {
            enemyName = "TankEnemy",
            spawnWeightByRound = new AnimationCurve(
                new Keyframe(1, 0),    // 0% in round 1
                new Keyframe(3, 10),   // 10% in round 3
                new Keyframe(6, 20),   // 20% by round 6
                new Keyframe(10, 30)   // 30% by round 10+
            )
        };
        
        // Default spawn weights for SoldierEnemy
        enemySpawnWeights[3] = new EnemySpawnWeight
        {
            enemyName = "SoldierEnemy",
            spawnWeightByRound = new AnimationCurve(
                new Keyframe(1, 0),    // 0% in round 1
                new Keyframe(2, 30),   // 30% in round 2
                new Keyframe(5, 40),   // 40% by round 5
                new Keyframe(10, 30)   // 30% by round 10+
            )
        };
    }

    public void SpawnInitialTurret() {
        //in game, may have meny to select initial turret to spawn, once get into game, read data from save, spawn specific turret
        //for now, to test, spawn 1 turret
        var turretPrefab = GetUnitByName<BaseTurret>("Turret1_0", Faction.Turret); //later this will be based on the selected turret from the initial turret selection UI
        var spawnedTurret = Instantiate(turretPrefab);
        var turretSpawnTile = GridManager.Instance.GetInitialTurretSpawnTile();

        turretSpawnTile.SetUnit(spawnedTurret);
        GameManager.Instance.ChangeState(GameState.PlayerPrepTurn);
    }

    public IEnumerator StartRoundLoop() {
        localNumberOfEnemiesToSpawn = GameManager.Instance.globalNumberOfEnemiesToSpawn;
        
        // Calculate spawn delay based on round number - enemies spawn faster in later rounds
        float spawnDelay = Mathf.Max(0.5f, 1.0f - (RoundManager.Instance.round * 0.05f));
        Debug.Log($"Round {RoundManager.Instance.round}: Spawn delay set to {spawnDelay} seconds");
        
        // Reset Titan spawn flag at the start of each round
        hasTitanSpawned = false;
        
        // Determine if a Titan should spawn this round
        bool shouldSpawnTitan = RoundManager.Instance.round >= firstTitanRound && 
                               (RoundManager.Instance.round % 5 == 0 || Random.value < 0.2f);
        
        while (localNumberOfEnemiesToSpawn > 0) {
            // Special case: Spawn a Titan if conditions are met
            if (shouldSpawnTitan && !hasTitanSpawned && localNumberOfEnemiesToSpawn <= GameManager.Instance.globalNumberOfEnemiesToSpawn / 2) {
                SpawnSpecificEnemy("TitanEnemy");
                hasTitanSpawned = true;
                localNumberOfEnemiesToSpawn--;
                
                // Give players a breather after the Titan
                yield return new WaitForSeconds(spawnDelay * 3);
                continue;
            }
            
            // Normal enemy spawn
            SpawnEnemy();
            localNumberOfEnemiesToSpawn--;
            yield return new WaitForSeconds(spawnDelay);
        }
        
        Debug.Log("Wave complete! All enemies spawned. Waiting for them to be defeated...");
    }

    public void BeginEnemyWave() {
        isWaveInProgress = true;
        enemyCount = 0; // Reset enemy count at the start of a wave
        StartCoroutine(StartRoundLoop());
        StartCoroutine(CheckForWaveCompletion());
    }
    
    // Periodically check if all enemies are defeated
    private IEnumerator CheckForWaveCompletion() {
        // Wait a bit to let enemies spawn
        yield return new WaitForSeconds(2f);
        
        while (isWaveInProgress) {
            // Check if all enemies have been spawned and defeated
            if (localNumberOfEnemiesToSpawn <= 0 && enemyCount <= 0) {
                Debug.Log("All enemies defeated! Returning to player prep turn.");
                isWaveInProgress = false;
                
                // Increment the round counter after completing a wave
                RoundManager.Instance.IncrementRound(1);
                
                GameManager.Instance.ChangeState(GameState.PlayerPrepTurn);
                break;
            }
            
            // Check every half second
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    // Determine if a specific enemy type should spawn based on current round
    private bool ShouldSpawnEnemyType(string enemyName) {
        int currentRound = RoundManager.Instance.round;
        
        // Special case for Titan
        if (enemyName == "TitanEnemy") {
            return currentRound >= firstTitanRound && !hasTitanSpawned;
        }
        
        // Get total weight of all possible enemies for this round
        float totalWeight = 0f;
        foreach (var enemyWeight in enemySpawnWeights) {
            totalWeight += enemyWeight.spawnWeightByRound.Evaluate(currentRound);
        }
        
        if (totalWeight <= 0) return false;
        
        // Get weight for the specific enemy
        float specificEnemyWeight = 0f;
        foreach (var enemyWeight in enemySpawnWeights) {
            if (enemyWeight.enemyName == enemyName) {
                specificEnemyWeight = enemyWeight.spawnWeightByRound.Evaluate(currentRound);
                break;
            }
        }
        
        // Calculate probability
        float probability = specificEnemyWeight / totalWeight;
        return Random.value < probability;
    }
    
    // Spawn a random enemy based on weights for the current round
    public void SpawnEnemy() {
        enemyCount++;
        
        int currentRound = RoundManager.Instance.round;
        
        // Calculate total weight for this round
        float totalWeight = 0f;
        foreach (var enemyWeight in enemySpawnWeights) {
            totalWeight += enemyWeight.spawnWeightByRound.Evaluate(currentRound);
        }
        
        // Select an enemy type based on weights
        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;
        string selectedEnemyName = "BlueStar"; // Default fallback
        
        foreach (var enemyWeight in enemySpawnWeights) {
            cumulativeWeight += enemyWeight.spawnWeightByRound.Evaluate(currentRound);
            if (randomValue <= cumulativeWeight) {
                selectedEnemyName = enemyWeight.enemyName;
                break;
            }
        }
        
        SpawnSpecificEnemy(selectedEnemyName);
    }
    
    // Spawn a specific enemy type
    public void SpawnSpecificEnemy(string enemyName) {
        try {
            var enemyPrefab = GetUnitByName<BaseEnemy>(enemyName, Faction.Enemy);
            var spawnedEnemy = Instantiate(enemyPrefab);
            var enemySpawnTile = GridManager.Instance.GetEnemySpawnTile();
            
            enemySpawnTile.SetUnit(spawnedEnemy);
            Debug.Log($"Spawned enemy: {enemyName}");
        }
        catch (System.Exception e) {
            Debug.LogError($"Failed to spawn enemy {enemyName}: {e.Message}");
            // Fallback to default enemy
            var enemyPrefab = GetUnitByName<BaseEnemy>("BlueStar", Faction.Enemy);
            var spawnedEnemy = Instantiate(enemyPrefab);
            var enemySpawnTile = GridManager.Instance.GetEnemySpawnTile();
            
            enemySpawnTile.SetUnit(spawnedEnemy);
            Debug.Log("Spawned fallback enemy: BlueStar");
        }
    }

    public List<BaseEnemy> GetAllCurrentEnemies() {
        var enemies = FindObjectsByType<BaseEnemy>(FindObjectsSortMode.None);
        return enemies.ToList();
    }

    public void SpawnEnemiesTest() {//spawns test enemies to configure pathfinding and turret projectiles with
        int testEnemySpawnCount = 20;
        Debug.Log("Spawn enemies test");
        for (int i = 0; i < testEnemySpawnCount; i++) { //maybe enemy count will increase as the game progresses (wave / round numbers increase)
            var enemyPrefab = GetUnitByName<BaseEnemy>("BlueStar", Faction.Enemy);
            var spawnedEnemy = Instantiate(enemyPrefab);
            //might also want to use a spawner here, but may be easier to just randomize under a range of positions since the spawn region is circular
            var enemySpawnTile = GridManager.Instance.GetEnemySpawnTileTest(); //

            enemySpawnTile.SetUnit(spawnedEnemy);
        }
    }

    public T GetUnitByName<T>(string unitName, Faction faction) where T : BaseUnit {
        var unit = _units.Where(u => u.Faction == faction && u.UnitPrefab.name == unitName).FirstOrDefault();
        if (unit == null) {
            Debug.LogWarning($"Unit not found: {unitName} with faction {faction} Using fallback unit.");
            // Return a fallback unit if available
            return (T)_units.Where(u => u.Faction == faction).FirstOrDefault()?.UnitPrefab;
        }
        return (T)unit.UnitPrefab;
    }

    public void SetSelectedUnit(BaseUnit unit) {
        // If we're selecting a turret that's not the central turret, deselect any previously selected turrets
        if (unit != null && unit.Faction == Faction.Turret) {
            // Deselect all other turrets first
            BaseTurret[] allTurrets = FindObjectsByType<BaseTurret>(FindObjectsSortMode.None);
            foreach (BaseTurret turret in allTurrets) {
                if (turret != unit) {
                    turret.SendMessage("SetSelected", false, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        
        SelectedUnit = unit; //maybe on canvas we display an image of the currently selected turret or something
        UIManager.Instance.ToggleShowSelectedUnit(unit);
    }
}
