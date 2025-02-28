using UnityEngine;

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
            } else if (OccupiedUnit.Faction == Faction.Block) {
                UnitManager.Instance.SetSelectedUnit((BaseBlock)OccupiedUnit);
            } else { //maybe add one for enemy, to display name (stats?) of enemy, def a later thing tho
                return; //because we only want to be able to click on turrets
            }
        } else { //OccupiedUnit is null
            //deselect current unit first
            UnitManager.Instance.SetSelectedUnit(null);
            
            //only attempt to place block if we have currency and it's player prep turn
            if (CurrencyManager.Instance.currency > 0) {
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
                    var floorPrefab = UnitManager.Instance.GetUnitByName<BaseBlock>("SandBlock", Faction.Block);
                    var spawnedFloor = Instantiate(floorPrefab);
                    var floorSpawnTile = GridManager.Instance.GetTileAtPosition(gridPos);
                    if (floorSpawnTile != null) {
                        floorSpawnTile.SetUnit(spawnedFloor);
                        
                        //mark this position as unwalkable in the pathfinding grid
                        Pathfinding.Instance.SetIsWalkable(gridPos, false);
                        
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
                        Debug.Log($"No tile found at position {gridPos}");
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
