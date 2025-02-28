using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance { get; private set; }
    
    [SerializeField] private int gridWidth = 20;
    [SerializeField] private int gridHeight = 20;
    
    private Pathfinding pathfinding;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePathfinding();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializePathfinding()
    {
        //use GridManager's dimensions if available
        if (GridManager.Instance != null)
        {
            gridWidth = GridManager.Instance._width;
            gridHeight = GridManager.Instance._height;
        }
        
        pathfinding = new Pathfinding(gridWidth, gridHeight);
        Debug.Log("Pathfinding initialized with grid size: " + gridWidth + "x" + gridHeight);
    }
    
    //helper method to check if pathfinding is ready
    public bool IsPathfindingReady()
    {
        return Pathfinding.Instance != null;
    }
} 