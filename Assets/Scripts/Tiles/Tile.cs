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
        _highlight.SetActive(true);
    }

    void OnMouseExit() {
        _highlight.SetActive(false);
    }

    void OnMouseDown() {
        if (GameManager.Instance.GameState != GameState.PlayerPrepTurn) {
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
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 gridPos = new Vector2(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));

            var floorPrefab = UnitManager.Instance.GetUnitByName<BaseBlock>("Sand_0", Faction.Block); //later this will be based on the selected turret from the initial turret selection UI, for now spawns tile
            var spawnedFloor = Instantiate(floorPrefab);
            var floorSpawnTile = GridManager.Instance.GetTileAtPosition(gridPos);
            if (floorSpawnTile != null) {
                floorSpawnTile.SetUnit(spawnedFloor);
            } else {
                Debug.Log($"No tile found at position {gridPos}");
            }

            //TODO: implement placement system
            //here, we probably want a selection system (if you're in place wall mode vs place turret mode, vs place nothing mode)
            //maybe q to rotate, e to change type, left click to place obviously
            //for now lets assume you default to no selection mode, so if there isn't a unit in the square you've selected, then set selection to null



            UnitManager.Instance.SetSelectedUnit(null);

            return; //return for now
            
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
}
