using UnityEngine;

public class PathNode
{
    private Grid<PathNode> grid;
    private int x;
    private int y;
    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return x + "," + y;
    }
}
