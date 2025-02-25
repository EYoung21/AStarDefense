using UnityEngine;

public class PathNode
{
    private Grid<PathNode> grid;
    private int x;
    private int y;

    public int gCost;
    public int fCost;
    public int hCost;

    public PathNode cameFromNode;
    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public void calculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return x + "," + y;
    }
}
