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

    void Awake() {
        Instance = this;


        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }

    public void SpawnInitialTurret() {

        //in game, may have meny to select initial turret to spawn, once get into game, read data from save, spawn specific turret

        //for now, to test, spawn 1 turret

        var turretPrefab = GetUnitByName<BaseTurret>("Turret1_0", Faction.Turret); //later this will be based on the selected turret from the initial turret selection UI
        var spawnedTurret = Instantiate(turretPrefab);
        var turretSpawnTile = GridManager.Instance.GetInitialTurretSpawnTile();

        turretSpawnTile.SetUnit(spawnedTurret);

        // spawnedTurret.OccupiedTile = turretSpawnTile;

        // GameManager.Instance.ChangeState(GameState.SpawnGameUI);

        GameManager.Instance.ChangeState(GameState.PlayerPrepTurn);
    }

    public IEnumerator StartRoundLoop() {
        localNumberOfEnemiesToSpawn = GameManager.Instance.globalNumberOfEnemiesToSpawn;
        while (localNumberOfEnemiesToSpawn > 0) {
            yield return new WaitForSeconds(1);
            SpawnEnemy();
            localNumberOfEnemiesToSpawn--;
        }
        
        //optional: Signal that the wave is complete
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
                GameManager.Instance.ChangeState(GameState.PlayerPrepTurn);
                break;
            }
            
            // Check every half second
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    public void SpawnEnemy() {
        enemyCount++;
        var enemyPrefab = GetUnitByName<BaseEnemy>("BlueStar", Faction.Enemy);
        var spawnedEnemy = Instantiate(enemyPrefab);
        //might also want to use a spawner here, but may be easier to just randomize under a range of positions since the spawn region is circular
        var enemySpawnTile = GridManager.Instance.GetEnemySpawnTile();
        // Debug.Log("spawn tile: enemySpawnTile");

        enemySpawnTile.SetUnit(spawnedEnemy);
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
        return (T)_units.Where(u => u.Faction == faction && u.UnitPrefab.name == unitName).First().UnitPrefab;
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
