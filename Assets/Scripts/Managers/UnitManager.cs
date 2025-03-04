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

    
    //enemy spawn weights by round
    [System.Serializable]
    public class EnemySpawnWeight
    {
        public string enemyName;
        public AnimationCurve spawnWeightByRound;
    }
    
    [Header("Enemy Spawn Settings")]
    //enemy spawn weights by round
    public EnemySpawnWeight[] enemySpawnWeights;
    public int firstTitanRound = 5;
    
    //track special enemy spawns
    private bool hasTitanSpawned = false;

    void Awake() {
        Instance = this;
        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
        
        //initialize default spawn weights if not set in inspector
        if (enemySpawnWeights == null || enemySpawnWeights.Length == 0)
        {
            InitializeDefaultSpawnWeights();
        }
    }
    
    private void InitializeDefaultSpawnWeights()
    {
        enemySpawnWeights = new EnemySpawnWeight[4];
        
        //default spawn weights for bluestar
        enemySpawnWeights[0] = new EnemySpawnWeight
        {
            enemyName = "BlueStar",
            spawnWeightByRound = new AnimationCurve(
                new Keyframe(1, 100),  //100% in round 1
                new Keyframe(3, 50),   //50% by round 3
                new Keyframe(6, 20),   //20% by round 6
                new Keyframe(10, 10)   //10% by round 10+
            )
        };
        
        //default spawn weights for SpeedEnemy
        enemySpawnWeights[1] = new EnemySpawnWeight
        {
            enemyName = "SpeedEnemy",
            spawnWeightByRound = new AnimationCurve(
                new Keyframe(1, 0),    //0% in round 1
                new Keyframe(2, 20),   //20% in round 2
                new Keyframe(5, 30),   //30% by round 5
                new Keyframe(10, 20)   //20% by round 10+
            )
        };
        
        //default spawn weights for TankEnemy
        enemySpawnWeights[2] = new EnemySpawnWeight
        {
            enemyName = "TankEnemy",
            spawnWeightByRound = new AnimationCurve(
                new Keyframe(1, 0),    //0% in round 1
                new Keyframe(3, 10),   //10% in round 3
                new Keyframe(6, 20),   //20% by round 6
                new Keyframe(10, 30)   //30% by round 10+
            )
        };
        
        //default spawn weights for SoldierEnemy
        enemySpawnWeights[3] = new EnemySpawnWeight
        {
            enemyName = "SoldierEnemy",
            spawnWeightByRound = new AnimationCurve(
                new Keyframe(1, 0),    //0% in round 1
                new Keyframe(2, 30),   //30% in round 2
                new Keyframe(5, 40),   //40% by round 5
                new Keyframe(10, 30)   //30% by round 10+
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
        
        //calculate spawn delay based on round number - enemies spawn faster in later rounds
        float spawnDelay = Mathf.Max(0.5f, 1.0f - (RoundManager.Instance.round * 0.05f));
        Debug.Log($"Round {RoundManager.Instance.round}: Spawn delay set to {spawnDelay} seconds");
        
        //reset Titan spawn flag at the start of each round
        hasTitanSpawned = false;
        
        //determine if a Titan should spawn this round
        bool shouldSpawnTitan = RoundManager.Instance.round >= firstTitanRound && 
                               (RoundManager.Instance.round % 5 == 0 || Random.value < 0.2f);
        
        while (localNumberOfEnemiesToSpawn > 0) {
            //special case: Spawn a Titan if conditions are met
            if (shouldSpawnTitan && !hasTitanSpawned && localNumberOfEnemiesToSpawn <= GameManager.Instance.globalNumberOfEnemiesToSpawn / 2) {
                SpawnSpecificEnemy("TitanEnemy");
                hasTitanSpawned = true;
                localNumberOfEnemiesToSpawn--;
                
                //give players a breather after the Titan
                yield return new WaitForSeconds(spawnDelay * 3);
                continue;
            }
            
            //normal enemy spawn
            SpawnEnemy();
            localNumberOfEnemiesToSpawn--;
            yield return new WaitForSeconds(spawnDelay);
        }
        
        Debug.Log("Wave complete! All enemies spawned. Waiting for them to be defeated...");
    }

    public void BeginEnemyWave() {
        isWaveInProgress = true;
        enemyCount = 0; //reset enemy count at the start of a wave
        
        //ensure turret targeting is enabled at the start of a wave
        SetTurretsTargetingActive(true);
        
        StartCoroutine(StartRoundLoop());
        StartCoroutine(CheckForWaveCompletion());
    }

    //periodically check if all enemies are defeated
    private IEnumerator CheckForWaveCompletion()
    {
        while (isWaveInProgress)
        {
            //wait until all enemies are defeated
            if (AllEnemiesDefeated())
            {
                isWaveInProgress = false;
                
                //immediately disable turret targeting to prevent random shooting
                SetTurretsTargetingActive(false);
                
                //award completion bonus if RoundManager exists
                if (RoundManager.Instance != null)
                {
                    int roundReward = RoundManager.Instance.GetRoundCompletionReward();
                    
                    //add round completion bonus
                    if (CurrencyManager.Instance != null)
                    {
                        Debug.Log($"<color=yellow>Round {RoundManager.Instance.round} completed! Bonus: {roundReward} currency</color>");
                        CurrencyManager.Instance.AddCurrency(roundReward);
                    }
                    
                    //increment round
                    RoundManager.Instance.IncrementRound(1);
                }
                
                //allow short pause between rounds
                yield return new WaitForSeconds(2.0f);
                
                //change game state back to player prep turn
                if (GameManager.Instance != null)
                {
                    Debug.Log("All enemies defeated! Returning to player prep turn.");
                    GameManager.Instance.ChangeState(GameState.PlayerPrepTurn);
                }
                
                //reset Titan spawn flag
                hasTitanSpawned = false;
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }

    //method to enable or disable turret targeting
    private void SetTurretsTargetingActive(bool active)
    {
        //find all turrets in the scene
        BaseTurret[] turrets = FindObjectsByType<BaseTurret>(FindObjectsSortMode.None);
        
        foreach (BaseTurret turret in turrets)
        {
            //call a method on BaseTurret to enable/disable targeting
            turret.SetTargetingActive(active);
        }
        
        Debug.Log($"Set turret targeting to {(active ? "active" : "inactive")}");
    }

    private bool AllEnemiesDefeated()
    {
        //return true if no enemies are alive (you can implement this depending on how your enemies are managed)
        var enemies = GetAllCurrentEnemies();
        return enemies.All(enemy => enemy == null || !enemy.IsAlive); //assumes IsAlive is a property of BaseEnemy
    }


    //determine if a specific enemy type should spawn based on current round
    private bool ShouldSpawnEnemyType(string enemyName) {
        int currentRound = RoundManager.Instance.round;
        
        //special case for Titan
        if (enemyName == "TitanEnemy") {
            return currentRound >= firstTitanRound && !hasTitanSpawned;
        }
        
        //get total weight of all possible enemies for this round
        float totalWeight = 0f;
        foreach (var enemyWeight in enemySpawnWeights) {
            totalWeight += enemyWeight.spawnWeightByRound.Evaluate(currentRound);
        }
        
        if (totalWeight <= 0) return false;
        
        //get weight for the specific enemy
        float specificEnemyWeight = 0f;
        foreach (var enemyWeight in enemySpawnWeights) {
            if (enemyWeight.enemyName == enemyName) {
                specificEnemyWeight = enemyWeight.spawnWeightByRound.Evaluate(currentRound);
                break;
            }
        }
        
        //calculate probability
        float probability = specificEnemyWeight / totalWeight;
        return Random.value < probability;
    }
    
    //spawn a random enemy based on weights for the current round
    public void SpawnEnemy() {
        enemyCount++;
        
        int currentRound = RoundManager.Instance.round;
        
        //calculate total weight for this round
        float totalWeight = 0f;
        foreach (var enemyWeight in enemySpawnWeights) {
            //only include enemies that we know exist
            if (enemyWeight.enemyName == "BlueStar" || 
                enemyWeight.enemyName == "SpeedEnemy" || 
                enemyWeight.enemyName == "TankEnemy" || 
                enemyWeight.enemyName == "SoldierEnemy" || 
                enemyWeight.enemyName == "TitanEnemy") {
                
                totalWeight += enemyWeight.spawnWeightByRound.Evaluate(currentRound);
            }
        }
        
        //select an enemy type based on weights
        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;
        string selectedEnemyName = "BlueStar"; //default fallback
        
        foreach (var enemyWeight in enemySpawnWeights) {
            //only consider enemies that we know exist
            if (enemyWeight.enemyName == "BlueStar" || 
                enemyWeight.enemyName == "SpeedEnemy" || 
                enemyWeight.enemyName == "TankEnemy" || 
                enemyWeight.enemyName == "SoldierEnemy" || 
                enemyWeight.enemyName == "TitanEnemy") {
                
                cumulativeWeight += enemyWeight.spawnWeightByRound.Evaluate(currentRound);
                if (randomValue <= cumulativeWeight) {
                    selectedEnemyName = enemyWeight.enemyName;
                    break;
                }
            }
        }
        
        //check if the selected enemy exists in our resources (extra safety check)
        bool enemyExists = _units.Any(u => u.Faction == Faction.Enemy && u.UnitPrefab.name == selectedEnemyName);
        
        //if the enemy doesn't exist, use the default fallback
        if (!enemyExists) {
            Debug.LogWarning($"Enemy {selectedEnemyName} not found in resources. Using fallback enemy BlueStar.");
            selectedEnemyName = "BlueStar";
        }
        
        SpawnSpecificEnemy(selectedEnemyName);
    }
    
    //spawn a specific enemy type
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
            //fallback to default enemy
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

    public T GetUnitByName<T>(string unitName, Faction faction) where T : BaseUnit {
        var unit = _units.Where(u => u.Faction == faction && u.UnitPrefab.name == unitName).FirstOrDefault();
        if (unit == null) {
            Debug.LogWarning($"Unit not found: {unitName} with faction {faction} Using fallback unit.");
            
            //for enemies, use BlueStar as the default fallback
            if (faction == Faction.Enemy) {
                var fallbackUnit = _units.Where(u => u.Faction == faction && u.UnitPrefab.name == "BlueStar").FirstOrDefault();
                if (fallbackUnit != null) {
                    return (T)fallbackUnit.UnitPrefab;
                }
            }
            
            //if specific fallback not found, use any unit of the same faction
            return (T)_units.Where(u => u.Faction == faction).FirstOrDefault()?.UnitPrefab;
        }
        return (T)unit.UnitPrefab;
    }

    public void SetSelectedUnit(BaseUnit unit) {
        //if we're selecting a turret that's not the central turret, deselect any previously selected turrets
        if (unit != null && unit.Faction == Faction.Turret) {
            //deselect all other turrets first
            BaseTurret[] allTurrets = FindObjectsByType<BaseTurret>(FindObjectsSortMode.None);
            foreach (BaseTurret turret in allTurrets) {
                if (turret != unit) {
                    turret.SendMessage("SetSelected", false, SendMessageOptions.DontRequireReceiver);
                }
            }
            
            //play turret selection sound
            if (SFXManager.Instance != null) {
                SFXManager.Instance.PlayTurretSelectionSound();
            }
        }
        
        SelectedUnit = unit; //maybe on canvas we display an image of the currently selected turret or something
        UIManager.Instance.ToggleShowSelectedUnit(unit);
    }

    public void OnEnemyDefeated()
    {
        enemyCount--;  //decrease the number of active enemies
    }
}
