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

    public int _width, _height;
    [SerializeField] private float _cellSize = 1f;
    [SerializeField] private Tile _floorTile, _blockTile;

    [SerializeField] private Transform _cam;

    private Dictionary<Vector2, Tile> _tiles;
    public Grid<bool> _grid;

    void Awake() {
        Instance = this;
        
        // Initialize the grid with a function that returns false (default walkable state)
        _grid = new Grid<bool>(_width, _height, _cellSize, Vector3.zero, (grid, x, y) => false);
    }


    public void GenerateGrid() {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {
                var spawnedTile = Instantiate(_floorTile, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.init(x, y);

                _tiles[new Vector2(x, y)] = spawnedTile;
                _grid.SetGridObject(x, y, false); // 0 means walkable

            }
        }

        _cam.transform.position = new Vector3((float)_width/2 - 0.5f, (float)_height/2 - 0.5f, -10);
        
        // Set camera size to fit height (36 tiles)
        Camera.main.orthographicSize = _height / 2f;  // This will make the height fit perfectly
        
        GameManager.Instance.ChangeState(GameState.SpawnInitialTurret);

        // foreach (var tile in _tiles)
        // {
        //     Debug.Log($"Key: {tile.Key}, Tile Name: {tile.Value.name}");
        // }
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

    public Tile GetEnemySpawnTile() {
        //create a list to hold potential spawn tiles (all edge tiles)
        List<Tile> edgeTiles = new List<Tile>();
        
        //add all edge tiles to the list
        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {
                //check if this is an edge tile
                if (x == 0 || x == _width - 1 || y == 0 || y == _height - 1) {
                    Vector2 pos = new Vector2(x, y);
                    if (_tiles.TryGetValue(pos, out Tile tile)) {
                        edgeTiles.Add(tile);
                    }
                }
            }
        }
        
        //if there are no edge tiles (shouldn't happen with a proper grid), log an error and return null
        if (edgeTiles.Count == 0) {
            Debug.LogError("No edge tiles found in the grid!");
            return null;
        }
        
        //return a random tile from the edge tiles
        return edgeTiles[Random.Range(0, edgeTiles.Count)];
    }
}
