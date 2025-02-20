using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    [SerializeField] private float _cellSize = 1f;
    [SerializeField] private Tile _floorTile, _blockTile;

    [SerializeField] private Transform _cam;

    private Dictionary<Vector2, Tile> _tiles;
    private Grid _grid;

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
        
        // Set camera size to fit height (36 tiles)
        Camera.main.orthographicSize = _height / 2f;  // This will make the height fit perfectly
        
        GameManager.Instance.ChangeState(GameState.SpawnInitialTurret);

        foreach (var tile in _tiles)
        {
            Debug.Log($"Key: {tile.Key}, Tile Name: {tile.Value.name}");
        }
    }
    
    public Tile GetTileAtPosition(Vector2 pos) {
        if (_tiles.TryGetValue(pos, out var tile)) {    
            return tile;
        }
        return null;
    }

    public Tile GetInitialTurretSpawnTile() { //this function should return the center of the grid (the location we want our initial turret, or it's prefab, to spawn on when the game starts)
        return _tiles.Where(t => t.Key.x == _width / 2).Where(t => t.Key.y == _height / 2).First().Value;
    }

    public Tile GetEnemySpawnTileTest() {
        return _tiles.OrderBy(t => Random.value).First().Value;
    }

    public Tile GetEnemySpawnTile() { // would be called every time an enemy is spawned.
        //would be outside a radius from the center of the grid
        //would be a random tile within that radius
        //would be a tile that is not already occupied by a turret
        //would be a tile that is not already occupied by an enemy
        //would be a tile that is not already occupied by a block

        //can figure out later, would likely be called every time an enemy is spawned
        throw new System.NotImplementedException();
    }
}
