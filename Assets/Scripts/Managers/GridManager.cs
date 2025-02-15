using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public static GridManager Instance;

    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _floorTile, _blockTile;

    [SerializeField] private Transform _cam;

    private Dictionary<Vector2, Tile> _tiles;

    void Awake() {
        Instance = this;
    }


    public void GenerateGrid() {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {
                var spawnedTile = Instantiate(_floorTile, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                spawnedTile.init(x, y);

                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        _cam.transform.position = new Vector3((float)_width/2 - 0.5f, (float)_height/2 - 0.5f, -10);

        GameManager.Instance.ChangeState(GameState.SpawnInitialTurret);
    }
    
    public Tile GetTileAtPosition(Vector2 pos) {
        if (_tiles.TryGetValue(pos, out var tile)) {
            return tile;
        }
        return null;
    }
}
