using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    
    [SerializeField] private int enemyCount;

    private List<ScriptableUnit> _units;

    public BaseUnit SelectedUnit;
    
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

        //maybe make another function later that can spawn additional turrets based on previous game save or played input
        // var turretCount = 1; //would be public
        // for (int i = 0; i < turretCount; i++) {
        //     //spawn turret
        // }
    }

    public void SpawnEnemies() {
        for (int i = 0; i < enemyCount; i++) { //maybe enemy count will increase as the game progresses (wave / round numbers increase)
            var enemyPrefab = GetUnitByName<BaseTurret>("Enemy1_0", Faction.Enemy);
            var spawnedEnemy = Instantiate(enemyPrefab);
            //might also want to use a spawner here, but may be easier to just randomize under a range of positions since the spawn region is circular
            var enemySpawnTile = GridManager.Instance.GetEnemySpawnTile();

            enemySpawnTile.SetUnit(spawnedEnemy);
        }
    }

    public T GetUnitByName<T>(string unitName, Faction faction) where T : BaseUnit {
        return (T)_units.Where(u => u.Faction == faction && u.UnitPrefab.name == unitName).First().UnitPrefab;
    }

    public void SetSelectedUnit(BaseUnit unit) {
        SelectedUnit = unit; //maybe on canvas we display an image of the currently selected turret or something
        UIManager.Instance.ToggleShowSelectedUnit(unit);
    }
}
