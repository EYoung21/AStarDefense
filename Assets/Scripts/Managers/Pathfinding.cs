using UnityEngine;

public class Pathfinding
{
    private Grid<PathNode> grid;
    public Pathfinding(int width, int height)
    {
        grid = new Grid<PathNode>(width, height, 1f, Vector3.zero, (Grid<PathNode> g, int x, int y) => new PathNode(g,x,y));
    }
    
}
