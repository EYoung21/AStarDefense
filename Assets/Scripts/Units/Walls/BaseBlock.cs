using UnityEngine;

public class BaseBlock : BaseUnit
{
    //start is called once before the first execution of update after the monobehaviour is created
    void Start()
    {
        
    }

    //update is called once per frame
    void Update()
    {
        
    }
    
    //forward mouse events to the occupied tile
    void OnMouseEnter() {
        if (OccupiedTile != null) {
            //call the tile's onmouseenter method
            OccupiedTile.SendMessage("OnMouseEnter", SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnMouseExit() {
        if (OccupiedTile != null) {
            //call the tile's onmouseexit method
            OccupiedTile.SendMessage("OnMouseExit", SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnMouseDown() {
        if (OccupiedTile != null) {
            //call the tile's onmousedown method
            OccupiedTile.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
        }
    }
}
