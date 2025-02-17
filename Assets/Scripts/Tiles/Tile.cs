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
        if (OccupiedUnit != null && OccupiedUnit.Faction == Faction.Turret || OccupiedUnit == null) { //if the tile has a turret on it or nothing on it.
            _highlight.SetActive(true);
        }
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
        //we will handle the unit's presence "leaving" the previous tile through this:

        if (unit.OccupiedTile != null) { //we need this check because the first time we spawn a unit, it's occupied tile will be null (don't want a null refrence exception)
            unit.OccupiedTile.OccupiedUnit = null; //sets the unit's current tile (the one before this one that the unit is moving to) to null
        }

        unit.transform.position = transform.position; //set's the unit's position to the position of the current tile
        OccupiedUnit = unit; //sets the refrence of the tile's current unit to the current unit
        unit.OccupiedTile = this; //set's the unit's occupied tile to the current tile (this)

    }
}
