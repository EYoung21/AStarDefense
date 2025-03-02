using UnityEngine;

public class FloorTile : Tile
{
    [SerializeField] private Color _baseColor, _offsetColor;


    public override void init(int x, int y) {
        var isOffset = (x+y) % 2 == 1;

        Color tileColor = isOffset ? _offsetColor : _baseColor;
        //tileColor.a = 0.1f; //force alpha (transparency level) to 1
        _renderer.color = tileColor;
    }
}
