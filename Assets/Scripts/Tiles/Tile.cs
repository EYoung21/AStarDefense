using UnityEngine;
using System.Collections.Generic;

public abstract class Tile : MonoBehaviour
{
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    [SerializeField] protected SpriteRenderer _renderer;
    //protected: effectively private, but derrived tiles (from this class) can also access it

    [SerializeField] private GameObject _highlight;

    public BaseUnit OccupiedUnit;
    
    //abstract class and virtual func combo!: allows this function to be defaulted to this in every instance, but if redefined will have logic specific to another tile
    public virtual void init(int x, int y) {
        
    }

    void OnMouseEnter() { //only want to set highlight for turrets, and places that aren't occupied by walls
        if (IsEdgeTile()) { //only want to allow highlighting if is player turn and not on edge
            return;
        }
        _highlight.SetActive(true);
    }

    void OnMouseExit() {
        _highlight.SetActive(false);
    }

    void OnMouseDown() {
        Debug.Log("Mouse down");

        if (IsEdgeTile()) {
            return;
        }

        if (OccupiedUnit != null) {
            //if it is a turret (not necessarily just the intiial turret as we may spawn more later) we're clicking on, we want to select it (maybe to upgrade it, destroy it,etc.)
            if (OccupiedUnit.Faction == Faction.Turret) {
                UnitManager.Instance.SetSelectedUnit((BaseTurret)OccupiedUnit); //typecasting to BaseTurret
                
                // Check if this is the central turret
                Vector2 centerPosition = new Vector2(
                    GridManager.Instance._width / 2, 
                    GridManager.Instance._height / 2
                );
                
                Vector2 currPosition = new Vector2(
                    Mathf.RoundToInt(OccupiedUnit.transform.position.x),
                    Mathf.RoundToInt(OccupiedUnit.transform.position.y)
                );
                
                if (centerPosition == currPosition) {
                    // Only allow selection for manual control during enemy wave turn
                    if (GameManager.Instance.GameState == GameState.EnemyWaveTurn) {
                        // Directly set the selected state on the turret
                        ((BaseTurret)OccupiedUnit).SetSelected(true);
                        Debug.Log("Central turret selected for manual control via tile");
                    }
                }
            } else if (OccupiedUnit.Faction == Faction.Block) {
                UnitManager.Instance.SetSelectedUnit((BaseBlock)OccupiedUnit);
            } else { //maybe add one for enemy, to display name (stats?) of enemy, def a later thing tho
                return; //because we only want to be able to click on turrets
            }
        } else { //OccupiedUnit is null
            //deselect current unit first
            UnitManager.Instance.SetSelectedUnit(null);
            
            // Check if we're in enemy wave turn and the central turret is selected
            bool canPlaceBlock = true;
            if (GameManager.Instance.GameState == GameState.EnemyWaveTurn) {
                // Find the central turret
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
                        // Check if the central turret is selected
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
            
            //only attempt to place block if we have currency, it's player prep turn or enemy wave turn with central turret not selected
            if (canPlaceBlock && CurrencyManager.Instance.currency > 0) {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 gridPos = new Vector2(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));
                
                //check if there's an enemy at this position
                Collider2D[] colliders = Physics2D.OverlapPointAll(gridPos);
                bool enemyPresent = false;
                
                foreach (Collider2D collider in colliders) {
                    //only check for BaseEnemy components, ignoring projectiles
                    if (collider.GetComponent<BaseEnemy>() != null) {
                        enemyPresent = true;
                        break;
                    }
                }
                
                if (!enemyPresent) {
                    //temporarily mark this position as unwalkable to check if it would block all paths
                    Pathfinding.Instance.SetIsWalkable(gridPos, false);
                    
                    //check if there's still at least one path from any edge to the center
                    bool pathExists = CheckPathFromEdgesToCenter();
                    
                    if (!pathExists) {
                        //if no path exists, revert the tile to walkable and prevent block placement
                        Pathfinding.Instance.SetIsWalkable(gridPos, true);
                        Debug.Log("Cannot place block here as it would block all paths to the center");
                        return;
                    }
                    
                    var floorPrefab = UnitManager.Instance.GetUnitByName<BaseBlock>("SandBlock", Faction.Block);
                    var spawnedFloor = Instantiate(floorPrefab);
                    var floorSpawnTile = GridManager.Instance.GetTileAtPosition(gridPos);
                    if (floorSpawnTile != null) {
                        floorSpawnTile.SetUnit(spawnedFloor);
                        
                        //we already marked this position as unwalkable above
                        
                        //force all enemies to recalculate their paths
                        BaseEnemy[] allEnemies = FindObjectsByType<BaseEnemy>(FindObjectsSortMode.None);
                        foreach (BaseEnemy enemy in allEnemies) {
                            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
                            if (movement != null) {
                                movement.RecalculatePath();
                            }
                        }
                        
                        GridManager.Instance._grid.SetGridObject(gridPos, true);
                        Debug.Log(GridManager.Instance._grid.ToString());
                    } else {
                        //revert the pathfinding grid if we couldn't find the tile
                        Pathfinding.Instance.SetIsWalkable(gridPos, true);
                        Debug.Log($"No tile found at position {gridPos}");
                        return;
                    }

                    CurrencyManager.Instance.currency -= 1;
                    UIManager.Instance.updateCurrencyUI();
                } else {
                    Debug.Log("Cannot place block on a square occupied by an enemy");
                }
            }
            return;
        }

        return; //?

        //TODO: add turret placement logic here
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
        // Get mouse position in world coordinates
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 gridPos = new Vector2(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));
        
        // Get the grid dimensions from GridManager
        int width = GridManager.Instance._width;
        int height = GridManager.Instance._height;
        
        // Check if the mouse position is on any edge of the grid
        return gridPos.x == 0 || gridPos.x == width - 1 || gridPos.y == 0 || gridPos.y == height - 1;
    }
}
