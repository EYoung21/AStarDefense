using UnityEngine;

public class FloorTile : Tile
{
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
           
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }


    [SerializeField] private Color _baseColor, _offsetColor;


    public override void init(int x, int y) {
        var isOffset = (x+y) % 2 == 1;

        Color tileColor = isOffset ? _offsetColor : _baseColor;
        tileColor.a = 1f; //force alpha (transparency level) to 1
        _renderer.color = tileColor;
    }
}
