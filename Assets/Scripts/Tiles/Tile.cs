using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public abstract class Tile : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer _renderer;
    //protected: effectively private, but derrived tiles (from this class) can also access it

    [SerializeField] private GameObject _highlight;

    public BaseUnit OccupiedUnit;
    
    //abstract class and virtual func combo!: allows this function to be defaulted to this in every instance, but if redefined will have logic specific to another tile
    public virtual void init(int x, int y) {
        
    }

    void OnMouseEnter() { //only want to set highlight for turrets, and places that aren't occupied by walls
        //don't highlight if over UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (IsEdgeTile()) { //only want to allow highlighting if is player turn and not on edge
            return;
        }
        
        //check if the central turret is selected for manual control
        bool centralTurretSelected = false;
        if (GameManager.Instance.GameState == GameState.EnemyWaveTurn) {
            //find the central turret
            Vector2 centerPosition = new Vector2(
                GridManager.Instance._width / 2, 
                GridManager.Instance._height / 2
            );
            
            BaseTurret[] allTurrets = FindObjectsByType<BaseTurret>(FindObjectsSortMode.None);
            foreach (BaseTurret turret in allTurrets) {
                Vector2 turretPos = new Vector2(
                    Mathf.RoundToInt(turret.transform.position.x),
                    Mathf.RoundToInt(turret.transform.position.y)
                );
                
                if (centerPosition == turretPos) {
                    //check if the central turret is selected
                    turret.GetSelectedState((state) => { centralTurretSelected = state; });
                    break;
                }
            }
        }
        
        //only show highlight if central turret is not selected for manual control
        if (!centralTurretSelected) {
            _highlight.SetActive(true);
        }
    }

    void OnMouseExit() {
        _highlight.SetActive(false);
    }

    void OnMouseDown() {
        Debug.Log("Mouse down");

        if (IsEdgeTile()) {
            return;
        }

        //handle turret selection and block placement
        if (OccupiedUnit != null) {
            if (OccupiedUnit.Faction == Faction.Turret) {
                UnitManager.Instance.SetSelectedUnit((BaseTurret)OccupiedUnit);
                
                //check if this is the central turret
                Vector2 centerPosition = new Vector2(
                    GridManager.Instance._width / 2, 
                    GridManager.Instance._height / 2
                );
                
                Vector2 currPosition = new Vector2(
                    Mathf.RoundToInt(OccupiedUnit.transform.position.x),
                    Mathf.RoundToInt(OccupiedUnit.transform.position.y)
                );
                
                if (GameManager.Instance.GameState == GameState.EnemyWaveTurn) {
                    //during enemy wave, only handle turret control
                    if (centerPosition == currPosition) {
                        ((BaseTurret)OccupiedUnit).SetSelected(true);
                        Debug.Log("Central turret selected for manual control via tile");
                    }
                } else if (GameManager.Instance.GameState == GameState.PlayerPrepTurn) {
                    //during prep turn, show upgrade panel
                    Debug.Log("Trying to show upgrade panel");
                    if (TurretUpgradeUI.Instance != null) {
                        Debug.Log("Found TurretUpgradeUI instance");
                        TurretUpgradeUI.Instance.ShowUpgradePanel(OccupiedUnit as BaseTurret);
                    } else {
                        Debug.LogError("TurretUpgradeUI.Instance is null! Make sure the UI is in the scene and the component is enabled.");
                    }
                }
            } else if (OccupiedUnit.Faction == Faction.Block) {
                UnitManager.Instance.SetSelectedUnit((BaseBlock)OccupiedUnit);
            }
            return;
        }

        //if the upgrade panel is active, just close it and don't place blocks
        if (TurretUpgradeUI.Instance != null && 
            TurretUpgradeUI.Instance.IsUpgradePanelActive())
        {
            TurretUpgradeUI.Instance.HideUpgradePanel();
            return;
        }

        //handle empty tile clicks
        UnitManager.Instance.SetSelectedUnit(null);
        
        //check if we can place blocks
        bool canPlaceBlock = true;
        
        //prevent block placement if a turret is selected during prep turn
        if (GameManager.Instance.GameState == GameState.PlayerPrepTurn && 
            UnitManager.Instance.SelectedUnit != null && 
            UnitManager.Instance.SelectedUnit is BaseTurret)
        {
            canPlaceBlock = false;
            Debug.Log("Cannot place blocks while a turret is selected");
            return;
        }
        
        //check for central turret control during enemy wave
        if (GameManager.Instance.GameState == GameState.EnemyWaveTurn) {
            Vector2 centerPosition = new Vector2(
                GridManager.Instance._width / 2, 
                GridManager.Instance._height / 2
            );
            
            BaseTurret[] allTurrets = FindObjectsByType<BaseTurret>(FindObjectsSortMode.None);
            foreach (BaseTurret turret in allTurrets) {
                Vector2 turretPos = new Vector2(
                    Mathf.RoundToInt(turret.transform.position.x),
                    Mathf.RoundToInt(turret.transform.position.y)
                );
                
                if (centerPosition == turretPos) {
                    bool isSelected = false;
                    turret.GetSelectedState((state) => { isSelected = state; });
                    if (isSelected) {
                        canPlaceBlock = false;
                        Debug.Log("Cannot place blocks while central turret is in manual control mode");
                        break;
                    }
                }
            }
        }

        //try to place block if allowed
        if (canPlaceBlock && CurrencyManager.Instance.CanAfford(5)) {
            PlaceBlock();
        }
    }

    private void PlaceBlock() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 gridPos = new Vector2(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));
        
        //check if there's an enemy at this position
        Collider2D[] colliders = Physics2D.OverlapPointAll(gridPos);
        bool enemyPresent = false;
        
        foreach (Collider2D collider in colliders) {
            if (collider.GetComponent<BaseEnemy>() != null) {
                enemyPresent = true;
                break;
            }
        }
        
        if (!enemyPresent) {
            Pathfinding.Instance.SetIsWalkable(gridPos, false);
            
            if (!CheckPathFromEdgesToCenter()) {
                Pathfinding.Instance.SetIsWalkable(gridPos, true);
                Debug.Log("Cannot place block here as it would block all paths to the center");
                return;
            }
            
            var floorPrefab = UnitManager.Instance.GetUnitByName<BaseBlock>("SandBlock", Faction.Block);
            var spawnedFloor = Instantiate(floorPrefab);
            var floorSpawnTile = GridManager.Instance.GetTileAtPosition(gridPos);
            
            if (floorSpawnTile != null) {
                floorSpawnTile.SetUnit(spawnedFloor);
                
                BaseEnemy[] allEnemies = FindObjectsByType<BaseEnemy>(FindObjectsSortMode.None);
                foreach (BaseEnemy enemy in allEnemies) {
                    EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
                    if (movement != null) {
                        movement.RecalculatePath();
                    }
                }
                
                GridManager.Instance._grid.SetGridObject(gridPos, true);
                Debug.Log(GridManager.Instance._grid.ToString());
                
                //play block placement sound
                if (SFXManager.Instance != null) {
                    SFXManager.Instance.PlayBlockPlacementSound();
                }
                
                CurrencyManager.Instance.RemoveCurrency(5);
                UIManager.Instance.updateCurrencyUI();
            } else {
                Pathfinding.Instance.SetIsWalkable(gridPos, true);
                Debug.Log($"No tile found at position {gridPos}");
            }
        } else {
            Debug.Log("Cannot place block on a square occupied by an enemy");
        }
    }

    void OnMouseUp() {

    }

    //check if there's at least one path from any edge to the center
    private bool CheckPathFromEdgesToCenter() {
        //get the center position
        int centerX = Mathf.FloorToInt(GridManager.Instance._width / 2);
        int centerY = Mathf.FloorToInt(GridManager.Instance._height / 2);
        Vector3 centerPos = new Vector3(centerX, centerY);
        
        //check all edge tiles
        int width = GridManager.Instance._width;
        int height = GridManager.Instance._height;
        
        //check top edge
        for (int x = 0; x < width; x++) {
            Vector3 edgePos = new Vector3(x, height - 1);
            List<Vector3> path = Pathfinding.Instance.FindPath(edgePos, centerPos);
            if (path != null && path.Count > 0) {
                return true; //found a path
            }
        }
        
        //check bottom edge
        for (int x = 0; x < width; x++) {
            Vector3 edgePos = new Vector3(x, 0);
            List<Vector3> path = Pathfinding.Instance.FindPath(edgePos, centerPos);
            if (path != null && path.Count > 0) {
                return true; //found a path
            }
        }
        
        //check left edge
        for (int y = 0; y < height; y++) {
            Vector3 edgePos = new Vector3(0, y);
            List<Vector3> path = Pathfinding.Instance.FindPath(edgePos, centerPos);
            if (path != null && path.Count > 0) {
                return true; //found a path
            }
        }
        
        //check right edge
        for (int y = 0; y < height; y++) {
            Vector3 edgePos = new Vector3(width - 1, y);
            List<Vector3> path = Pathfinding.Instance.FindPath(edgePos, centerPos);
            if (path != null && path.Count > 0) {
                return true; //found a path
            }
        }
        
        //no path found from any edge to the center
        return false;
    }

    public void SetUnit(BaseUnit unit) {
        if (unit.OccupiedTile != null) {
            unit.OccupiedTile.OccupiedUnit = null;
        }
        
        unit.transform.position = transform.position;
        
        //get sprite renderers
        SpriteRenderer tileSprite = GetComponent<SpriteRenderer>();
        SpriteRenderer unitSprite = unit.GetComponent<SpriteRenderer>();
        
        if (tileSprite != null && unitSprite != null) {
            //calculate scale factors
            float scaleX = tileSprite.bounds.size.x / unitSprite.bounds.size.x;
            float scaleY = tileSprite.bounds.size.y / unitSprite.bounds.size.y;
            
            //apply the scale
            unit.transform.localScale = new Vector3(scaleX, scaleY, 1f);            
        }
        
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }

    private bool IsEdgeTile() {
        //get mouse position in world coordinates
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 gridPos = new Vector2(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));
        
        //get the grid dimensions from GridManager
        int width = GridManager.Instance._width;
        int height = GridManager.Instance._height;
        
        //check if the mouse position is on any edge of the grid
        return gridPos.x == 0 || gridPos.x == width - 1 || gridPos.y == 0 || gridPos.y == height - 1;
    }
}
